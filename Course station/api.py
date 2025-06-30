# --- NLTK punkt_tab PATCH (MUST BE FIRST) ---
import builtins
import importlib

def patched_nltk_data_load(resource_name, *args, **kwargs):
    if 'punkt_tab' in resource_name:
        print('[PATCH] Redirecting NLTK resource punkt_tab to punkt')
        resource_name = resource_name.replace('punkt_tab', 'punkt')
    try:
        return builtins._original_nltk_data_load(resource_name, *args, **kwargs)
    except Exception as e:
        # Fallback: if punkt_tab fails, try punkt directly
        if 'punkt_tab' in resource_name:
            print('[PATCH] Fallback: loading punkt directly')
            resource_name = resource_name.replace('punkt_tab', 'punkt')
            return builtins._original_nltk_data_load(resource_name, *args, **kwargs)
        raise e

import nltk.data
if not hasattr(builtins, '_original_nltk_data_load'):
    builtins._original_nltk_data_load = nltk.data.load
    nltk.data.load = patched_nltk_data_load

# Ensure punkt is downloaded at startup (safe, will not re-download if present)
import nltk
try:
    nltk.data.find('tokenizers/punkt')
except LookupError:
    nltk.download('punkt')
print('[PATCH] NLTK punkt_tab patch is active')
# --- END PATCH ---

import openai
import json
import numpy as np
import time
import random
import re
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from fastapi.middleware.cors import CORSMiddleware
from rapidfuzz import fuzz
import io
import contextlib
from typing import Optional, List
from threading import Lock
import string
import difflib
import nltk
from nltk.corpus import stopwords
from nltk.stem import PorterStemmer
import ast
from nltk.corpus import wordnet
import logging
from fastapi.responses import JSONResponse
from fastapi import Request
import traceback
import os
from dotenv import load_dotenv
import os
from dotenv import load_dotenv
nltk.download('stopwords')
nltk.download('wordnet')

# ====== LOAD DATA AND EMBEDDINGS =======
with open('book.json', 'r', encoding='utf-8') as f:
    book_data = json.load(f)

def extract_tagged_questions_from_book(book_data, n_easy=30, n_medium=40, n_hard=30):
    import random
    easy_mcq, easy_tf = [], []
    medium, hard = [], []
    medium_mcq, medium_tf = [], []
    hard_mcq, hard_tf = [], []
    tf_keywords = [' is ', ' are ', ' was ', ' were ']
    for chapter in book_data.get('chapters', []):
        content = chapter.get('content', '')
        title = chapter.get('title', '')
        sentences = [s.strip() for s in content.split('\n') if len(s.strip()) > 30]
        for s in sentences:
            # Easy: Only MCQ and T/F questions
            if any(kw in s.lower() for kw in tf_keywords):
                # T/F for easy - randomly make some False by negating the statement
                is_true = random.choice([True, False])
                statement = s
                if not is_true:
                    # Create a false statement by adding "not" or negating the sentence
                    if " is " in s.lower():
                        statement = s.replace(" is ", " is not ")
                    elif " are " in s.lower():
                        statement = s.replace(" are ", " are not ")
                    elif " was " in s.lower():
                        statement = s.replace(" was ", " was not ")
                    elif " were " in s.lower():
                        statement = s.replace(" were ", " were not ")
                    else:
                        # If we can't negate easily, just make it true
                        is_true = True
                
                easy_tf.append({
                    'type': 'tf',
                    'topic': title,
                    'statement': statement,
                    'correct_answer': 'True' if is_true else 'False',
                    'explanation': f'This statement is {"" if is_true else "not "}taken directly from the chapter: {title}.',
                    'difficulty': 'easy'
                })
                # Occasionally add MCQ for easy
                if random.random() < 0.08:
                    options = [s, 'Distractor 1', 'Distractor 2', 'Distractor 3']
                    random.shuffle(options)
                    correct_letter = chr(65 + options.index(s))
                    easy_mcq.append({
                        'type': 'mcq',
                        'topic': title,
                        'question': f'Which of the following is correct: "{s[:60]}..."?',
                        'options': options,
                        'correct_answer': correct_letter,
                        'explanation': f'The correct answer is based on the actual book content.',
                        'difficulty': 'easy'
                    })
            # Medium: shorter sentences (<= 100 chars)
            if len(s) <= 100:
                medium.append({
                    'type': 'general',
                    'topic': title,
                    'question': f'What is the meaning of: "{s[:60]}..."?',
                    'answer': s,
                    'explanation': f'This is a direct excerpt from the chapter: {title}.',
                    'difficulty': 'medium'
                })
                if random.random() < 0.05:
                    # T/F for medium - randomly make some False
                    is_true = random.choice([True, False])
                    statement = s
                    if not is_true:
                        # Create a false statement by adding "not" or negating the sentence
                        if " is " in s.lower():
                            statement = s.replace(" is ", " is not ")
                        elif " are " in s.lower():
                            statement = s.replace(" are ", " are not ")
                        elif " was " in s.lower():
                            statement = s.replace(" was ", " was not ")
                        elif " were " in s.lower():
                            statement = s.replace(" were ", " were not ")
                        else:
                            # If we can't negate easily, just make it true
                            is_true = True
                    
                    medium_tf.append({
                        'type': 'tf',
                        'topic': title,
                        'statement': statement,
                        'correct_answer': 'True' if is_true else 'False',
                        'explanation': f'This statement is {"" if is_true else "not "}taken directly from the chapter: {title}.',
                        'difficulty': 'medium'
                    })
            # Hard: longer sentences (> 100 chars)
            if len(s) > 100:
                hard.append({
                    'type': 'general',
                    'topic': title,
                    'question': f'Explain the following: "{s[:60]}..."',
                    'answer': s,
                    'explanation': f'This is a direct excerpt from the chapter: {title}.',
                    'difficulty': 'hard'
                })
                if random.random() < 0.05:
                    # T/F for hard - randomly make some False
                    is_true = random.choice([True, False])
                    statement = s
                    if not is_true:
                        # Create a false statement by adding "not" or negating the sentence
                        if " is " in s.lower():
                            statement = s.replace(" is ", " is not ")
                        elif " are " in s.lower():
                            statement = s.replace(" are ", " are not ")
                        elif " was " in s.lower():
                            statement = s.replace(" was ", " was not ")
                        elif " were " in s.lower():
                            statement = s.replace(" were ", " were not ")
                        else:
                            # If we can't negate easily, just make it true
                            is_true = True
                    
                    hard_tf.append({
                        'type': 'tf',
                        'topic': title,
                        'statement': statement,
                        'correct_answer': 'True' if is_true else 'False',
                        'explanation': f'This statement is {"" if is_true else "not "}taken directly from the chapter: {title}.',
                        'difficulty': 'hard'
                    })
    random.shuffle(easy_mcq)
    random.shuffle(easy_tf)
    random.shuffle(medium)
    random.shuffle(medium_mcq)
    random.shuffle(medium_tf)
    random.shuffle(hard)
    random.shuffle(hard_mcq)
    random.shuffle(hard_tf)
    # Limit MCQ and TF for easy to 15 each (total 30 easy)
    easy = easy_mcq[:15] + easy_tf[:15]
    # Limit MCQ and TF for medium and hard to 2 each
    medium = medium[:n_medium-4] + medium_mcq[:2] + medium_tf[:2]
    hard = hard[:n_hard-4] + hard_mcq[:2] + hard_tf[:2]
    return (easy[:n_easy] + medium[:n_medium] + hard[:n_hard])

# Use this as the knowledge base
kb = extract_tagged_questions_from_book(book_data, n_easy=30, n_medium=40, n_hard=30)

def chunk_to_string(chunk):
    s = ""
    if 'type' in chunk:
        s += f"[{chunk['type'].upper()}] "
    if 'topic' in chunk:
        s += f"Topic: {chunk['topic']}. "
    if 'question' in chunk:
        s += f"Q: {chunk['question']} "
    if 'answer' in chunk:
        s += f"A: {chunk['answer']} "
    if 'explanation' in chunk:
        s += f"Explanation: {chunk['explanation']}"
    return s.strip()

corpus = [chunk_to_string(item) for item in kb]

openai.api_key = "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"  # <--- PUT YOUR VALID KEY HERE

def embed_texts_batched(texts, batch_size=100):
    embeddings = []
    for i in range(0, len(texts), batch_size):
        batch = [t for t in texts[i:i+batch_size] if t.strip()]
        if not batch:
            continue
        try:
            response = openai.embeddings.create(
                input=batch,
                model="text-embedding-3-small"
            )
            batch_embeds = [d.embedding for d in response.data]
            embeddings.extend(batch_embeds)
            time.sleep(0.5)
        except Exception as e:
            print(f"Batch {i} failed: {e}")
    return embeddings       

print("Embedding knowledge base (may take a while on first run)...")
corpus_embeddings = embed_texts_batched(corpus, batch_size=100)
print(f"Total embeddings: {len(corpus_embeddings)}")

def cosine_similarity(a, b):
    a = np.array(a)
    b = np.array(b)
    return np.dot(a, b) / (np.linalg.norm(a) * np.linalg.norm(b))

def retrieve_relevant_chunks_by_topic(topic, kb, corpus, corpus_embeddings, top_k=3):
    query_embedding = embed_texts_batched([topic])[0]
    sims = [cosine_similarity(query_embedding, emb) for emb in corpus_embeddings]
    top_indices = np.argsort(sims)[-top_k:][::-1]
    return [kb[i] for i in top_indices]

# Leaderboard storage (in-memory for demo)
leaderboard_easy = []
leaderboard_medium = []
leaderboard_hard = []
leaderboard_lock = Lock()

class QuestionRequest(BaseModel):
    topic: str
    difficulty: Optional[str] = None  # 'easy', 'medium', 'hard'
    used_questions: Optional[List[str]] = None  # List of question texts already used in this quiz

class AnswerCheckRequest(BaseModel):
    user_answer: str
    correct_answer: str

class LeaderboardEntry(BaseModel):
    username: str
    score: int
    difficulty: str  # 'easy', 'medium', 'hard'

class HintRequest(BaseModel):
    question_type: str  # 'mcq', 'general', 'coding', 'tf'
    question: str
    answer: str
    options: Optional[list] = None  # for MCQ

def is_duplicate_question(new_q, used_questions, threshold=85):
    # Compare new_q (string) to each in used_questions, return True if similar
    if not used_questions:
        return False
    for uq in used_questions:
        if fuzz.ratio(new_q.strip().lower(), uq.strip().lower()) >= threshold:
            return True
    return False

# Helper to extract the main question text from a KB or LLM output
def extract_question_text(q, qtype):
    if isinstance(q, dict):
        if qtype == "tf":
            return q.get("statement", "")
        return q.get("question", "")
    # LLM output string
    m = re.search(r"Question:\s*(.+?)(\n|$)", q, re.DOTALL)
    if m:
        return m.group(1).strip()
    return q.strip()

# Improved LLM prompt helpers

def make_general_question_prompt(topic, source_text, difficulty=None, used_questions=None):
    diff_str = f" (Difficulty: {difficulty})" if difficulty else ""
    used_str = "" if not used_questions else ("\n\nDo NOT repeat any of these questions (or their meaning):\n" + "\n".join(used_questions))
    return (
        f"Based ONLY on the following context about {topic}{diff_str}, generate ONE clear, unique, exam-appropriate general question and its answer. {used_str}\n\n"
        f"Context:\n{source_text}\n\n"
        f"Format:\nQuestion: ...\nAnswer: ..."
    )

def make_mcq_question_prompt(topic, source_text, difficulty=None, used_questions=None):
    diff_str = f" (Difficulty: {difficulty})" if difficulty else ""
    used_str = "" if not used_questions else ("\n\nDo NOT repeat any of these questions (or their meaning):\n" + "\n".join(used_questions))
    return (
        f"Based ONLY on the following context about {topic}{diff_str}, generate ONE clear, unique, exam-appropriate multiple-choice question (MCQ) with 4 options (A-D). IMPORTANT: Make sure to randomly choose the correct answer (A, B, C, or D with approximately equal distribution). Indicate the correct answer, and provide a brief explanation. {used_str}\n\n"
        f"Context:\n{source_text}\n\n"
        f"Format:\n"
        f"Question: ...\n"
        f"A. ...\nB. ...\nC. ...\nD. ...\n"
        f"Correct Answer: ...\n"
        f"Explanation: ..."
    )

def make_coding_question_prompt(topic, source_text, difficulty=None, used_questions=None):
    diff_str = f" (Difficulty: {difficulty})" if difficulty else ""
    used_str = "" if not used_questions else ("\n\nDo NOT repeat any of these questions (or their meaning):\n" + "\n".join(used_questions))
    return (
        f"Based ONLY on the following context about {topic}{diff_str}, generate ONE clear, unique, exam-appropriate coding question with its answer for an exam. {used_str}\n\n"
        f"Context:\n{source_text}\n\n"
        f"Format:\nQuestion: ...\nAnswer: ..."
    )

def make_tf_question_prompt(topic, source_text, difficulty=None, used_questions=None):
    diff_str = f" (Difficulty: {difficulty})" if difficulty else ""
    used_str = "" if not used_questions else ("\n\nDo NOT repeat any of these questions (or their meaning):\n" + "\n".join(used_questions))
    return (
        f"Based ONLY on the following context about {topic}{diff_str}, generate ONE clear, unique, exam-appropriate true/false (T/F) question. IMPORTANT: Make sure to randomly choose whether the answer is True or False (approximately 50% True and 50% False). Provide the correct answer (True or False) and a brief explanation. {used_str}\n\n"
        f"Context:\n{source_text}\n\n"
        f"Format:\nQuestion: ...\nAnswer: ...\nExplanation: ..."
    )

def generate_auto_question(prompt, max_tokens=256):
    print(f"[DEBUG] Generating question with prompt:\n{prompt}\n---")
    try:
        response = openai.chat.completions.create(
            model="gpt-4o",
            messages=[
                {"role": "system", "content": "You are a helpful question generation assistant for data structures exams."},
                {"role": "user", "content": prompt}
            ],
            max_tokens=max_tokens,
            temperature=0.7
        )
        result = response.choices[0].message.content.strip()
        print(f"[DEBUG] Generated question:\n{result}\n=====")
        return result
    except Exception as e:
        print(f"[DEBUG] OpenAI API error: {e}")
        return ""  # Return empty string on error

def generate_unique_questions(topic, question_type, n, kb, corpus, corpus_embeddings, max_tokens=256, difficulty=None, used_questions=None):
    used_chunks = set()
    questions = []
    attempts = 0
    while len(questions) < n and attempts < n*5:
        relevant_chunks = retrieve_relevant_chunks_by_topic(topic, kb, corpus, corpus_embeddings, top_k=5)
        random.shuffle(relevant_chunks)
        for chunk in relevant_chunks:
            chunk_str = chunk_to_string(chunk)
            if chunk_str in used_chunks:
                continue
            used_chunks.add(chunk_str)
            if question_type == "mcq":
                prompt = make_mcq_question_prompt(topic, chunk_str, difficulty, used_questions)
            elif question_type == "coding":
                prompt = make_coding_question_prompt(topic, chunk_str, difficulty, used_questions)
            elif question_type == "tf":
                prompt = make_tf_question_prompt(topic, chunk_str, difficulty, used_questions)
            else:
                prompt = make_general_question_prompt(topic, chunk_str, difficulty, used_questions)
            q = generate_auto_question(prompt, max_tokens=max_tokens)
            if not q or not isinstance(q, str) or len(q.strip()) == 0:
                print("[DEBUG] Skipping empty question from LLM.")
                continue
            question_text = extract_question_text(q, question_type)
            if is_duplicate_question(question_text, used_questions):
                print(f"[DEBUG] Skipping duplicate question: {question_text}")
                continue
            if all(question_text.lower() not in x.lower() for x in questions):
                questions.append(q)
                break
        attempts += 1
    print(f"[DEBUG] generate_unique_questions returning: {questions}")
    return questions

def semantic_similarity(a, b):
    # Use OpenAI embeddings (or any other embedding model)
    try:
        # Truncate very long inputs to avoid API errors
        max_len = 2000
        a_trunc = a[:max_len]
        b_trunc = b[:max_len]
        response = openai.embeddings.create(
            input=[a_trunc, b_trunc],
            model="text-embedding-3-small"
        )
        if not hasattr(response, 'data') or len(response.data) < 2:
            print(f"Semantic similarity error: Unexpected response format: {response}")
            return 0.0
        emb_a = response.data[0].embedding
        emb_b = response.data[1].embedding
        return cosine_similarity(emb_a, emb_b)
    except Exception as e:
        print(f"Semantic similarity error: {e} (a_len={len(a)}, b_len={len(b)})")
        return 0.0

def get_synonyms(word):
    synonyms = set()
    for syn in wordnet.synsets(word):
        for lemma in syn.lemmas():
            synonyms.add(lemma.name())
    return synonyms

def normalize_text(text):
    # Lowercase, strip, remove punctuation, remove stopwords, stem, and expand synonyms
    text = text.strip().lower().translate(str.maketrans('', '', string.punctuation))
    tokens = safe_word_tokenize(text)
    stop_words = set(stopwords.words('english'))
    ps = PorterStemmer()
    filtered = [ps.stem(w) for w in tokens if w not in stop_words]
    # Expand with synonyms
    expanded = set(filtered)
    for w in filtered:
        expanded.update([ps.stem(s) for s in get_synonyms(w)])
    return ' '.join(expanded)

def normalize_code(code):
    # Remove comments and extra whitespace
    import re
    # Remove single-line comments
    code = re.sub(r'#.*', '', code)
    # Remove multi-line comments (triple quotes)
    code = re.sub(r'"""[\s\S]*?"""|\'\'\'[\s\S]*?\'\'\'', '', code)
    # Remove extra whitespace and blank lines
    code = '\n'.join(line.strip() for line in code.splitlines() if line.strip())
    return code

def ast_equal(code1, code2):
    try:
        tree1 = ast.parse(code1)
        tree2 = ast.parse(code2)
        return ast.dump(tree1) == ast.dump(tree2)
    except Exception as e:
        print(f"[DEBUG] AST comparison error: {e}")
        return False

def extract_keywords(text):
    # Lowercase, remove punctuation, tokenize, remove stopwords, stem
    text = text.strip().lower().translate(str.maketrans('', '', string.punctuation))
    tokens = safe_word_tokenize(text)
    stop_words = set(stopwords.words('english'))
    ps = PorterStemmer()
    keywords = set(ps.stem(w) for w in tokens if w not in stop_words)
    return keywords

def extract_concepts(text):
    # Extracts key concepts (nouns, important words) from text
    tokens = safe_word_tokenize(text.lower())
    stop_words = set(stopwords.words('english'))
    concepts = set([w for w in tokens if w.isalpha() and w not in stop_words and len(w) > 2])
    return concepts

def is_answer_correct(user_answer, correct_answer, threshold=85, semantic_threshold=0.6, keyword_overlap_ratio=0.7):
    # Normalize answers
    user_answer_norm = normalize_text(user_answer)
    correct_answer_norm = normalize_text(correct_answer)
    # If the answer is empty or too short, just return False, but do not treat as error
    if not user_answer_norm or len(user_answer_norm.split()) < 5:
        debug_info = {
            'user_answer_norm': user_answer_norm,
            'correct_answer_norm': correct_answer_norm,
            'fuzzy': 0,
            'semantic': 0,
            'difflib': 0,
            'keyword_overlap': False,
            'partial_keyword_overlap': 0.0,
            'concept_overlap': 0.0,
            'substring_match': False,
            'reason': 'empty or too short',
            'is_concept_ok': False
        }
        return False, debug_info
    # Substring/superstring match only if answer is long enough
    user_raw = re.sub(r'[^a-zA-Z0-9]', '', user_answer.lower())
    correct_raw = re.sub(r'[^a-zA-Z0-9]', '', correct_answer.lower())
    
    # Check if user's answer is too short or contains no actual text content
    if len(user_raw) < 10 or not re.search(r'[a-zA-Z]', user_answer):
        debug_info = {
            'user_answer_norm': user_answer_norm,
            'correct_answer_norm': correct_answer_norm,
            'fuzzy': 0,
            'semantic': 0,
            'difflib': 0,
            'keyword_overlap': False,
            'partial_keyword_overlap': 0.0,
            'concept_overlap': 0.0,
            'substring_match': False,
            'reason': 'answer too short or only punctuation',
            'is_concept_ok': False
        }
        return False, debug_info
    
    min_len = int(0.8 * max(len(user_raw), 1))
    if (user_raw in correct_raw or correct_raw in user_raw) and len(user_raw) >= min_len and len(user_raw) > 0:
        return True, {'substring_match': True, 'reason': 'substring/superstring match'}
    # Fuzzy, semantic, and difflib scores
    fuzzy_score = fuzz.ratio(user_answer_norm, correct_answer_norm)
    sem_score = semantic_similarity(user_answer_norm, correct_answer_norm)
    diff_score = difflib.SequenceMatcher(None, user_answer_norm, correct_answer_norm).ratio()
    # Keyword and concept overlap
    user_keywords = extract_keywords(user_answer)
    correct_keywords = extract_keywords(correct_answer)
    user_concepts = extract_concepts(user_answer)
    correct_concepts = extract_concepts(correct_answer)
    if len(correct_keywords) == 0:
        keyword_overlap = False
        partial_overlap = 0.0
    else:
        overlap_count = len(user_keywords & correct_keywords)
        partial_overlap = overlap_count / len(correct_keywords)
        keyword_overlap = partial_overlap >= keyword_overlap_ratio
    partial_keyword_match = partial_overlap >= 0.5
    # Concept overlap: accept if user answer contains at least 70% of the key concepts
    if len(correct_concepts) > 0:
        concept_overlap = len(user_concepts & correct_concepts) / len(correct_concepts)
    else:
        concept_overlap = 0.0
    is_concept_ok = concept_overlap >= 0.7
    # Accept only if at least one strict criterion is met
    is_correct = (
        fuzzy_score >= threshold or
        sem_score >= semantic_threshold or
        diff_score >= 0.75 or
        keyword_overlap or
        is_concept_ok
    )
    debug_info = {
        'user_answer_norm': user_answer_norm,
        'correct_answer_norm': correct_answer_norm,
        'fuzzy': fuzzy_score,
        'semantic': sem_score,
        'difflib': diff_score,
        'keyword_overlap': keyword_overlap,
        'partial_keyword_overlap': partial_overlap,
        'concept_overlap': concept_overlap,
        'substring_match': False,
        'reason': 'score/overlap' if is_correct else 'no match',
        'is_concept_ok': is_concept_ok
    }
    return is_correct, debug_info


def extract_names_from_code(code):
    try:
        tree = ast.parse(code)
        class_names = {node.name for node in ast.walk(tree) if isinstance(node, ast.ClassDef)}
        func_names = {node.name for node in ast.walk(tree) if isinstance(node, ast.FunctionDef)}
        return class_names, func_names
    except Exception as e:
        print(f"[DEBUG] Name extraction error: {e}")
        return set(), set()

def extract_code_signatures(code):
    # Extract class and function signatures from code
    try:
        tree = ast.parse(code)
        classes = [node.name for node in ast.walk(tree) if isinstance(node, ast.ClassDef)]
        funcs = [node.name for node in ast.walk(tree) if isinstance(node, ast.FunctionDef)]
        return set(classes), set(funcs)
    except Exception:
        return set(), set()

def is_code_semantically_correct(user_code, correct_code, threshold=85, semantic_threshold=0.6, structure_overlap_ratio=0.7):
    debug_info = {}
    # Reject empty or too-short code
    if not user_code.strip() or len(user_code.strip().splitlines()) < 2:
        debug_info['reason'] = 'empty or too short'
        return False, debug_info
    # 1. AST comparison
    try:
        if ast_equal(user_code, correct_code):
            debug_info['ast_equal'] = True
            debug_info['reason'] = 'AST match'
            return True, debug_info
        else:
            debug_info['ast_equal'] = False
    except Exception as e:
        debug_info['ast_equal'] = False
        debug_info['ast_error'] = str(e)
    # 2. Normalize code
    user_code_norm = normalize_code(user_code)
    correct_code_norm = normalize_code(correct_code)
    # 3. Substring/superstring check only if code is long enough
    min_len = int(0.8 * max(len(correct_code_norm.replace(' ', '')), 1))
    if (user_code_norm.replace(' ', '') in correct_code_norm.replace(' ', '') or correct_code_norm.replace(' ', '') in user_code_norm.replace(' ', '')) and len(user_code_norm.replace(' ', '')) >= min_len and len(user_code_norm.replace(' ', '')) > 0:
        debug_info['substring_match'] = True
        debug_info['reason'] = 'Substring/superstring match'
        return True, debug_info
    else:
        debug_info['substring_match'] = False
    # 4. Output comparison
    user_output = safe_exec(user_code)
    correct_output = safe_exec(correct_code)
    if user_output and correct_output:
        user_output_norm = normalize_text(user_output)
        correct_output_norm = normalize_text(correct_output)
        debug_info['user_output'] = user_output_norm
        debug_info['correct_output'] = correct_output_norm
        if user_output_norm == correct_output_norm or user_output_norm.replace(' ', '') == correct_output_norm.replace(' ', ''):
            debug_info['output_match'] = True
            debug_info['reason'] = 'Output match'
            return True, debug_info
        else:
            debug_info['output_match'] = False
    # 5. Fuzzy, semantic, difflib
    fuzzy_score = fuzz.ratio(user_code_norm, correct_code_norm)
    sem_score = semantic_similarity(user_code_norm, correct_code_norm)
    diff_score = difflib.SequenceMatcher(None, user_code_norm, correct_code_norm).ratio()
    debug_info['fuzzy'] = fuzzy_score
    debug_info['semantic'] = sem_score
    debug_info['difflib'] = diff_score
    # 6. Structure/concept overlap
    user_classes, user_funcs = extract_code_signatures(user_code)
    correct_classes, correct_funcs = extract_code_signatures(correct_code)
    class_overlap = len(user_classes & correct_classes) / max(1, len(correct_classes)) if correct_classes else 0
    func_overlap = len(user_funcs & correct_funcs) / max(1, len(correct_funcs)) if correct_funcs else 0
    structure_overlap = (class_overlap + func_overlap) / 2
    debug_info['class_overlap'] = class_overlap
    debug_info['func_overlap'] = func_overlap
    debug_info['structure_overlap'] = structure_overlap
    all_classes_ok = correct_classes.issubset(user_classes)
    all_funcs_ok = correct_funcs.issubset(user_funcs)
    debug_info['all_classes_ok'] = bool(all_classes_ok)
    debug_info['all_funcs_ok'] = bool(all_funcs_ok)
    # 7. Concept overlap
    user_concepts = extract_concepts(user_code)
    correct_concepts = extract_concepts(correct_code)
    concept_overlap = len(user_concepts & correct_concepts) / max(1, len(correct_concepts)) if correct_concepts else 0
    debug_info['concept_overlap'] = concept_overlap
    # Accept only if at least one strict criterion is met
    is_correct = (
        fuzzy_score >= threshold or
        sem_score >= semantic_threshold or
        diff_score >= 0.75 or
        structure_overlap >= structure_overlap_ratio or
        concept_overlap >= 0.7 or
        bool(all_classes_ok) or
        bool(all_funcs_ok)
    )
    debug_info['reason'] = 'score/overlap' if is_correct else 'no match'
    return bool(is_correct), debug_info

def parse_media_from_text(text):
    # Simple demo: look for [image:url], [audio:url], [video:url] in the text
    media = None
    img_match = re.search(r'\[image:(.+?)\]', text)
    if img_match:
        media = {"type": "image", "url": img_match.group(1).strip()}
    aud_match = re.search(r'\[audio:(.+?)\]', text)
    if aud_match:
        media = {"type": "audio", "url": aud_match.group(1).strip()}
    vid_match = re.search(r'\[video:(.+?)\]', text)
    if vid_match:
        media = {"type": "video", "url": vid_match.group(1).strip()}
    return media

# Helper: Filter out unsuitable book contexts
META_KEYWORDS = [
    'summary', 'preface', 'about', 'donation', 'runestone', 'support', 'free materials', 'our mission', 'introduction', 'acknowledgment', 'foreword', 'copyright'
]
def is_suitable_context(topic, question_text=None):
    topic_l = topic.lower() if topic else ''
    if any(k in topic_l for k in META_KEYWORDS):
        return False
    if question_text and any(k in question_text.lower() for k in META_KEYWORDS):
        return False
    return True

# Helper: Stricter validation for generated questions
FORBIDDEN_WORDS = [
    'runestone academy', 'donation', 'supporting educational resources',
    'free materials', 'consider a $10 donation', 'our mission', 'support from individuals',
    'chapter', 'summary', 'preface', 'about', 'introduction', 'foreword', 'acknowledgment', 'copyright'
]
def is_valid_question(text):
    if not text or len(text.split()) < 7:
        return False
    text_l = text.lower()
    if any(word in text_l for word in FORBIDDEN_WORDS):
        return False
    # No meta references
    if 'this chapter' in text_l or 'as discussed' in text_l or 'in this section' in text_l:
        return False
    return True

def are_valid_mcq_options(options):
    if not options or len(options) != 4:
        return False
    # All options must be unique, plausible, and not meta
    seen = set()
    for opt in options:
        if not opt or len(opt.strip()) < 2:
            return False
        if any(word in opt.lower() for word in FORBIDDEN_WORDS):
            return False
        if opt in seen:
            return False
        seen.add(opt)
    return True

def is_real_mcq_options(options):
    # Returns True if all options are not placeholders
    if not options or len(options) != 4:
        return False
    for opt in options:
        if not opt or 'distractor' in opt.lower():
            return False
    return True

# ============== FASTAPI ================
app = FastAPI(title="Question Generation API")
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Or restrict to your frontend URL for more security
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.exception_handler(Exception)
async def all_exception_handler(request: Request, exc: Exception):
    print("Exception occurred:", exc)
    traceback.print_exc()
    return JSONResponse(
        status_code=500,
        content={"detail": str(exc)},
    )

# --- HINT ENDPOINT ---
@app.post("/hint")
def get_hint(request: HintRequest):
    try:
        logging.debug(f"[HINT REQUEST] {request}")
        hint = "No hint available."
        if request.question_type == "mcq" and request.options:
            correct = request.answer.strip()
            # Accept both letter and text as correct answer
            correct_letter = None
            correct_text = None
            # If correct is a single letter (A/B/C/D), map to option
            if len(correct) == 1 and correct.upper() in ['A', 'B', 'C', 'D']:
                idx = ord(correct.upper()) - 65
                if 0 <= idx < len(request.options):
                    correct_text = request.options[idx].strip()
                    correct_letter = correct.upper()
            else:
                correct_text = correct
            # Eliminate only a wrong option
            wrong_options = []
            for idx, opt in enumerate(request.options):
                opt_stripped = opt.strip()
                letter = chr(65 + idx)
                if (correct_letter and letter == correct_letter) or (correct_text and opt_stripped == correct_text):
                    continue
                wrong_options.append(opt)
            if wrong_options:
                eliminated = random.choice(wrong_options)
                hint = f"One wrong option is: {eliminated}"
            else:
                hint = "No hint available."
        elif request.question_type == "coding":
            # Only show function/class signatures or a high-level comment, never code logic or output
            lines = [line for line in request.answer.strip().splitlines() if line.strip()]
            sig_lines = [line for line in lines if line.strip().startswith(('def ', 'class '))]
            comment_lines = [line for line in lines if line.strip().startswith('#')]
            if sig_lines:
                hint = "Hint: " + ' | '.join(sig_lines)
            elif comment_lines:
                hint = "Hint: " + comment_lines[0]
            elif lines:
                # Show only the first 5 words of the first line
                words = lines[0].split()
                hint = "Hint: " + ' '.join(words[:5]) + ("..." if len(words) > 5 else "")
        elif request.question_type == "general":
            # Show only the first 7-10 words, never the full answer
            answer = request.answer.strip()
            if not answer and hasattr(request, 'explanation') and request.explanation:
                answer = request.explanation.strip()
            words = answer.split()
            if words:
                hint = "Hint: " + " ".join(words[:10]) + ("..." if len(words) > 10 else "")
        elif request.question_type == "tf":
            # Only give a vague clue, not the answer
            if request.answer.strip():
                hint = f"The answer starts with: {request.answer.strip()[0]}"
        logging.debug(f"[HINT RESPONSE] {hint}")
        return {"hint": hint}
    except Exception as e:
        logging.error(f"[HINT ERROR] {e}")
        return {"hint": "No hint available."}

# --- LEADERBOARD ENDPOINTS ---
@app.post("/leaderboard/submit")
def submit_score(entry: LeaderboardEntry):
    try:
        with leaderboard_lock:
            if entry.difficulty == 'easy':
                lb = leaderboard_easy
            elif entry.difficulty == 'medium':
                lb = leaderboard_medium
            elif entry.difficulty == 'hard':
                lb = leaderboard_hard
            else:
                raise HTTPException(status_code=400, detail="Invalid difficulty")
            lb.append({"username": entry.username, "score": entry.score})
            lb.sort(key=lambda x: x["score"], reverse=True)
            lb[:] = lb[:10]  # Keep top 10
        return {"msg": "Score submitted."}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

@app.get("/leaderboard/top")
def get_leaderboard(difficulty: str):
    try:
        with leaderboard_lock:
            if difficulty == 'easy':
                lb = leaderboard_easy
            elif difficulty == 'medium':
                lb = leaderboard_medium
            elif difficulty == 'hard':
                lb = leaderboard_hard
            else:
                raise HTTPException(status_code=400, detail="Invalid difficulty")
            return {"leaderboard": lb}
    except Exception as e:
        raise HTTPException(status_code=500, detail=str(e))

def is_good_question(q):
    # For MCQ and general: check 'question', for TF: check 'statement', for coding: check 'question'
    text = q.get("question", "") or q.get("statement", "")
    # Must be at least 7 words and end with ? or .
    return text and len(text.split()) > 6 and (text.strip().endswith("?") or text.strip().endswith("."))

@app.post("/generate/general")
def generate_general(request: QuestionRequest):
    print("[DEBUG] /generate/general request:", request)
    try:
        max_attempts = 5
        for attempt in range(max_attempts):
            questions = generate_unique_questions(
                request.topic, "general", 1, kb, corpus, corpus_embeddings, difficulty=request.difficulty, used_questions=request.used_questions
            )
            print("[DEBUG] Generated questions (general):", questions)
            if not questions or not isinstance(questions, list) or len(questions) == 0:
                print("[DEBUG] No questions generated by LLM for /generate/general, falling back to KB.")
                break
            q = questions[0]
            print("[DEBUG] Raw LLM output (q):", q)
            question_match = re.search(r"Question:\s*(.+?)\nAnswer:", q, re.DOTALL)
            answer_match = re.search(r"Answer:\s*(.+)", q, re.DOTALL)
            explanation_match = re.search(r"Explanation:\s*(.+)", q)
            media = parse_media_from_text(q)
            if not (media and media.get('type') == 'image' and media.get('url', '').startswith('http')):
                media = None
            question_text = question_match.group(1).strip() if question_match else q
            answer_val = (answer_match.group(1).strip() if answer_match else '')
            if not answer_val and explanation_match:
                answer_val = explanation_match.group(1).strip()
            explanation = explanation_match.group(1).strip() if explanation_match else ''
            # Filter out forbidden/off-topic questions
            if is_valid_question(question_text) and is_valid_question(answer_val):
                return {
                    "Question": question_text,
                    "Answer": answer_val,
                    "Explanation": explanation,
                    "Media": media
                }
            print(f"[DEBUG] Invalid or off-topic question, regenerating (attempt {attempt+1})...")
        # Fallback to KB
        filtered_kb = [q for q in kb if q.get('difficulty') == request.difficulty and q['type'] == 'general']
        good_kb = [q for q in filtered_kb if is_good_question(q) and not is_duplicate_question(q.get('question', ''), request.used_questions) and is_valid_question(q.get('question', ''))]
        if good_kb:
            q = random.choice(good_kb)
            print("[DEBUG] Serving KB general question:", q)
            return {
                "Question": q.get("question", ""),
                "Answer": q.get("answer", ""),
                "Explanation": q.get("explanation", ""),
                "Media": None
            }
        return {"Question": "", "Answer": "", "Explanation": ""}
    except Exception as e:
        print("[DEBUG] Exception in /generate/general:", e)
        # Never raise HTTPException here; always return a valid response
        return {"Question": "", "Answer": "", "Explanation": ""}

@app.post("/generate/mcq")
def generate_mcq(request: QuestionRequest):
    print("[DEBUG] /generate/mcq request:", request)
    try:
        if request.difficulty == 'hard':
            # Do not allow MCQ for hard difficulty
            return {"Question": "", "Options": [], "CorrectAnswer": ""}
        max_attempts = 5
        for attempt in range(max_attempts):
            questions = generate_unique_questions(
                request.topic, "mcq", 1, kb, corpus, corpus_embeddings, difficulty=request.difficulty, used_questions=request.used_questions
            )
            print("[DEBUG] Generated questions (mcq):", questions)
            if not questions or not isinstance(questions, list) or len(questions) == 0:
                print("[DEBUG] No questions generated by LLM for /generate/mcq, falling back to KB.")
                break
            q = questions[0]
            print("[DEBUG] Raw LLM output (q):", q)
            question_match = re.search(r"Question:\s*(.+?)\nA\.", q, re.DOTALL)
            options_match = re.findall(r"[A-D]\.\s*(.+)", q)
            correct_match = re.search(r"Correct Answer:\s*([A-D])", q)
            correct_text_match = re.search(r"Correct Answer:\s*(.+)", q)
            explanation_match = re.search(r"Explanation:\s*(.+)", q)
            media = parse_media_from_text(q)
            if not (media and media.get('type') == 'image' and media.get('url', '').startswith('http')):
                media = None
            question_text = question_match.group(1).strip() if question_match else q
            # Remove duplicate options and filter for validity
            options = options_match[:4] if options_match and len(options_match) >= 4 else []
            options = list(dict.fromkeys(options))  # Remove duplicates
            options = [opt for opt in options if is_valid_question(opt)]
            while len(options) < 4:
                options.append(f"Option {len(options)+1}")
            options = options[:4]
            correct_answer = ""
            if correct_match:
                correct_answer = correct_match.group(1)
            elif correct_text_match:
                correct_answer = correct_text_match.group(1).strip()
            else:
                correct_answer = ""
            explanation = explanation_match.group(1).strip() if explanation_match else ""
            # Filter out forbidden/off-topic questions and ensure 4 unique options
            if is_valid_question(question_text) and len(set(options)) == 4 and all(is_valid_question(opt) for opt in options):
                return {
                    "Question": question_text,
                    "Options": options,
                    "CorrectAnswer": correct_answer,
                    "Explanation": explanation,
                    "Media": media
                }
            print(f"[DEBUG] Invalid or off-topic MCQ or options, regenerating (attempt {attempt+1})...")
        # Fallback to KB
        filtered_kb = [q for q in kb if q.get('difficulty') == request.difficulty and q['type'] == 'mcq']
        good_kb = [
            q for q in filtered_kb
            if is_good_question(q)
            and not is_duplicate_question(q.get('question', ''), request.used_questions)
            and is_valid_question(q.get('question', ''))
            and is_real_mcq_options(q.get('options', []))
        ]
        if good_kb:
            q = random.choice(good_kb)
            print("[DEBUG] Serving KB MCQ:", q)
            return {
                "Question": q.get("question", ""),
                "Options": q.get("options", []),
                "CorrectAnswer": q.get("correct_answer", ""),
                "Explanation": q.get("explanation", ""),
                "Media": None
            }
        return {"Question": "", "Options": [], "CorrectAnswer": ""}
    except Exception as e:
        print("[DEBUG] Exception in /generate/mcq:", e)
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/generate/truefalse")
def generate_truefalse(request: QuestionRequest):
    print("[DEBUG] /generate/truefalse request:", request)
    try:
        if request.difficulty == 'hard':
            # Do not allow TF for hard difficulty
            return {"Statement": "", "CorrectAnswer": ""}
        max_attempts = 5
        for attempt in range(max_attempts):
            questions = generate_unique_questions(
                request.topic, "tf", 1, kb, corpus, corpus_embeddings, difficulty=request.difficulty, used_questions=request.used_questions
            )
            print("[DEBUG] Generated questions (truefalse):", questions)
            if not questions or not isinstance(questions, list) or len(questions) == 0:
                print("[DEBUG] No questions generated by LLM for /generate/truefalse, falling back to KB.")
                break
            q = questions[0]
            print("[DEBUG] Raw LLM output (q):", q)
            statement_match = re.search(r"Question:\s*(.+?)\nAnswer:", q, re.DOTALL)
            answer_match = re.search(r"Answer:\s*(True|False)", q)
            explanation_match = re.search(r"Explanation:\s*(.+)", q)
            media = parse_media_from_text(q)
            if not (media and media.get('type') == 'image' and media.get('url', '').startswith('http')):
                media = None
            statement = statement_match.group(1).strip() if statement_match else q
            correct_answer = answer_match.group(1) if answer_match else ""
            explanation = explanation_match.group(1).strip() if explanation_match else ""
            # Filter out forbidden/off-topic questions
            if is_valid_question(statement):
                return {
                    "Statement": statement,
                    "CorrectAnswer": correct_answer,
                    "Explanation": explanation,
                    "Media": media
                }
            print(f"[DEBUG] Invalid or off-topic TF question, regenerating (attempt {attempt+1})...")
        # Fallback to KB
        filtered_kb = [q for q in kb if q.get('difficulty') == request.difficulty and q['type'] == 'tf']
        good_kb = [q for q in filtered_kb if is_good_question(q) and not is_duplicate_question(q.get('statement', ''), request.used_questions) and is_valid_question(q.get('statement', ''))]
        if good_kb:
            q = random.choice(good_kb)
            print("[DEBUG] Serving KB TF:", q)
            return {
                "Statement": q.get("statement", ""),
                "CorrectAnswer": q.get("correct_answer", ""),
                "Explanation": q.get("explanation", ""),
                "Media": None
            }
        return {"Statement": "", "CorrectAnswer": ""}
    except Exception as e:
        print("[DEBUG] Exception in /generate/truefalse:", e)
        raise HTTPException(status_code=500, detail=str(e))

@app.post("/generate/coding")
def generate_coding(request: QuestionRequest):
    print("[DEBUG] /generate/coding request:", request)
    try:
        max_attempts = 5
        for attempt in range(max_attempts):
            questions = generate_unique_questions(
                request.topic, "coding", 1, kb, corpus, corpus_embeddings, difficulty=request.difficulty, used_questions=request.used_questions
            )
            print("[DEBUG] Generated questions (coding):", questions)
            if not questions or not isinstance(questions, list) or len(questions) == 0:
                print("[DEBUG] No questions generated by LLM for /generate/coding, falling back to KB.")
                break
            q = questions[0]
            print("[DEBUG] Raw LLM output (q):", q)
            question_match = re.search(r"Question:\s*(.+?)\nAnswer:", q, re.DOTALL)
            answer_match = re.search(r"Answer:\s*(.+)", q, re.DOTALL)
            explanation_match = re.search(r"Explanation:\s*(.+)", q)
            media = parse_media_from_text(q)
            if not (media and media.get('type') == 'image' and media.get('url', '').startswith('http')):
                media = None
            question_text = question_match.group(1).strip() if question_match else q
            answer_val = (answer_match.group(1).strip() if answer_match else '')
            if not answer_val and explanation_match:
                answer_val = explanation_match.group(1).strip()
            explanation = explanation_match.group(1).strip() if explanation_match else ''
            # Filter out forbidden/off-topic questions
            if is_valid_question(question_text):
                return {
                    "Question": question_text,
                    "Answer": answer_val,
                    "Explanation": explanation,
                    "Media": media
                }
            print(f"[DEBUG] Invalid or off-topic coding question, regenerating (attempt {attempt+1})...")
        # Fallback to KB
        filtered_kb = [q for q in kb if q.get('difficulty') == request.difficulty and q['type'] == 'coding']
        good_kb = [q for q in filtered_kb if is_good_question(q) and not is_duplicate_question(q.get('question', ''), request.used_questions) and is_valid_question(q.get('question', ''))]
        if good_kb:
            q = random.choice(good_kb)
            print("[DEBUG] Serving KB coding question:", q)
            return {
                "Question": q.get("question", ""),
                "Answer": q.get("answer", ""),
                "Explanation": q.get("explanation", ""),
                "Media": None
            }
        return {"Question": "", "Answer": "", "Explanation": ""}
    except Exception as e:
        print("[DEBUG] Exception in /generate/coding:", e)
        raise HTTPException(status_code=500, detail=str(e))

def normalize_code_for_comparison(code):
    # Remove print statements
    code = re.sub(r'print\s*\(.*?\)', '', code)
    # Remove comments
    code = re.sub(r'#.*', '', code)
    # Remove docstrings (triple quotes)
    code = re.sub(r'""".*?"""|\'\'\'.*?\'\'\'', '', code, flags=re.DOTALL)
    # Lowercase, remove extra whitespace
    code = code.lower()
    code = re.sub(r'\s+', '', code)
    return code

def is_code_similar(user_code, correct_code, threshold=0.6):
    user_code_clean = normalize_code_for_comparison(user_code)
    correct_code_clean = normalize_code_for_comparison(correct_code)
    similarity = difflib.SequenceMatcher(None, user_code_clean, correct_code_clean).ratio()
    return similarity >= threshold, similarity

@app.post("/check/general")
def check_general_answer(request: AnswerCheckRequest):
    try:
        user_answer = request.user_answer
        correct_answer = request.correct_answer
        print(f"[DEBUG] /check/general user_answer: {user_answer!r}, correct_answer: {correct_answer!r}")
        is_correct, scores = is_answer_correct(user_answer, correct_answer)
        print(f"[DEBUG] /check/general is_correct: {is_correct}, scores: {scores}")
        logging.debug(f"[CHECK GENERAL] user: {user_answer} | correct: {correct_answer} | result: {is_correct} | scores: {scores}")
        result = {"is_correct": bool(is_correct), "scores": scores}
        return ensure_native_bools(result)
    except Exception as e:
        import traceback
        print(f"[ERROR] Exception in /check/general: {e}")
        traceback.print_exc()
        logging.error(f"[CHECK GENERAL ERROR] {e}")
        return ensure_native_bools({"error": f"Error checking answer. Please try again. Exception: {e}"})

@app.post("/check/coding")
def check_coding_answer(payload: dict):
    try:
        user_answer = payload.get('user_answer', '')
        correct_answer = payload.get('correct_answer', '')
        is_correct, debug_info = is_code_semantically_correct(user_answer, correct_answer)
        return ensure_native_bools({
            "is_correct": bool(is_correct),
            "debug": debug_info
        })
    except Exception as e:
        logging.error(f"[CHECK CODING ERROR] {e}")
        return ensure_native_bools({"error": "Error checking code answer. Please try again."})

@app.get("/")
def root():
    return {"msg": "Question Generation API is running."}

# Add safe_sent_tokenize and safe_word_tokenize wrappers if not present

def safe_sent_tokenize(text):
    try:
        from nltk.tokenize import sent_tokenize
        return sent_tokenize(text)
    except Exception as e:
        # Only log, do not print or propagate unless fallback also fails
        logging.debug(f'[NLTK sent_tokenize fallback] {e}')
        # Fallback: split on period
        return [s.strip() for s in text.split('.') if s.strip()]

def safe_word_tokenize(text):
    try:
        from nltk.tokenize import word_tokenize
        return word_tokenize(text)
    except Exception as e:
        # Only log, do not print or propagate unless fallback also fails
        logging.debug(f'[NLTK word_tokenize fallback] {e}')
        # Fallback: split on whitespace
        return text.split()

# --- Safe code execution for answer checking ---
import io
import contextlib

def safe_exec(code):
    """Executes code and returns stdout output as string."""
    try:
        output = io.StringIO()
        with contextlib.redirect_stdout(output):
            exec(code, {})
        return output.getvalue().strip()
    except Exception as e:
        return str(e)

# --- Utility: Ensure all bools are native Python bool ---
def ensure_native_bools(obj):
    """Recursively convert all numpy.bool_ to native bool in dicts/lists/tuples/sets."""
    try:
        import numpy as np
    except ImportError:
        np = None
    if isinstance(obj, dict):
        return {k: ensure_native_bools(v) for k, v in obj.items()}
    elif isinstance(obj, list):
        return [ensure_native_bools(v) for v in obj]
    elif isinstance(obj, tuple):
        return tuple(ensure_native_bools(v) for v in obj)
    elif isinstance(obj, set):
        return {ensure_native_bools(v) for v in obj}
    elif np and isinstance(obj, np.bool_):
        return bool(obj)
    elif type(obj).__name__ == 'bool_' and hasattr(obj, 'item'):
        return bool(obj.item())
    return obj