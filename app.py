# # api.py

# from fastapi import FastAPI
# from fastapi.middleware.cors import CORSMiddleware
# from pydantic import BaseModel
# from typing import List, Dict
# import random

# # Hugging Face imports
# from transformers import AutoTokenizer, AutoModelForCausalLM, logging as hf_logging

# # silence verbose HF logs
# hf_logging.set_verbosity_error()

# app = FastAPI()
# app.add_middleware(
#     CORSMiddleware,
#     allow_origins=["*"],  # adjust in production
#     allow_methods=["*"],
#     allow_headers=["*"],
# )

# # --- Load your model & tokenizer once ---
# MODEL_NAME = "deepseek-ai/deepseek-coder-1.3b-instruct"
# tokenizer = AutoTokenizer.from_pretrained(MODEL_NAME, trust_remote_code=True)
# model     = AutoModelForCausalLM.from_pretrained(MODEL_NAME, trust_remote_code=True)

# # --- Request schemas ---
# class TextRequest(BaseModel):
#     text: str

# class CodeRequest(BaseModel):
#     code: str

# # --- Response schemas ---
# class GeneralQuestionResponse(BaseModel):
#     question: str

# class MCQResponse(BaseModel):
#     question: str
#     options: List[str]
#     correct_answer: str

# class TrueFalseResponse(BaseModel):
#     statement: str
#     correct_answer: str

# class CodingQuestionResponse(BaseModel):
#     question: str

# # --- Generation functions (your Colab logic, slightly adapted) ---

# def generate_general_question(text: str, max_length: int = 150) -> str:
#     prompt = (
#         "Generate one clear question that can be answered using ONLY this text:\n"
#         f"Text: {text}\n"
#         "Question:"
#     )
#     inputs = tokenizer(prompt, return_tensors="pt")
#     outputs = model.generate(
#         **inputs,
#         max_length=max_length,
#         do_sample=True,
#         temperature=0.7,
#         top_k=50
#     )
#     out = tokenizer.decode(outputs[0], skip_special_tokens=True)
#     return out.split("Question:")[-1].strip()

# def generate_mcq_question(text: str, max_new_tokens: int = 150) -> Dict:
#     prompt = (
#         "Create a multiple-choice question with 4 options (A-D) and correct answer.\n"
#         f"Text: {text}\n"
#         "Question:"
#     )
#     inputs = tokenizer(prompt, return_tensors="pt")
#     outputs = model.generate(
#         **inputs,
#         max_new_tokens=max_new_tokens,
#         do_sample=True,
#         temperature=0.7,
#         top_p=0.9,
#         pad_token_id=tokenizer.eos_token_id,
#         eos_token_id=tokenizer.eos_token_id,
#     )
#     full = tokenizer.decode(outputs[0], skip_special_tokens=True).strip()
#     gen = full[len(prompt):].strip()

#     # attempt to split into question/options/answer
#     # assume format: Q\nA) ...\nB) ...\nC) ...\nD) ...\nAnswer: X
#     lines = [l.strip() for l in gen.replace("\r", "").split("\n") if l.strip()]
#     # fallback if no line breaks
#     if len(lines) == 1:
#         for sep in ["A)", "B)", "C)", "D)", "Answer:"]:
#             lines = gen.replace(sep, f"\n{sep}").split("\n")
#         lines = [l.strip() for l in lines if l.strip()]

#     # parse
#     question = lines[0]
#     opts = [lines[i][2:].strip() for i in range(1,5) if i < len(lines)]
#     ans  = ""
#     for l in lines:
#         if l.lower().startswith("answer"):
#             ans = l.split(":",1)[1].strip()
#     return {"question": question, "options": opts, "correct_answer": ans}

# def generate_true_false_question(text: str) -> Dict:
#     make_false = random.choice([True, False])
#     label = "False" if make_false else "True"
#     prompt = (
#         f"Generate a {label} statement about the following text.\n"
#         f"Text: {text}\n"
#         "Statement:"
#     )
#     inputs = tokenizer(prompt, return_tensors="pt")
#     outputs = model.generate(
#         **inputs,
#         max_length=100,
#         do_sample=True,
#         temperature=0.7,
#         pad_token_id=tokenizer.eos_token_id,
#         eos_token_id=tokenizer.eos_token_id,
#     )
#     out = tokenizer.decode(outputs[0], skip_special_tokens=True).strip()
#     stmt = out.split("Statement:")[-1].strip()
#     return {"statement": stmt, "correct_answer": label}

# def generate_coding_question(code: str, max_length: int = 200) -> str:
#     prompt = (
#         "Generate a coding question about this code:\n"
#         f"Code:\n{code}\n"
#         "Question:"
#     )
#     inputs = tokenizer(prompt, return_tensors="pt")
#     outputs = model.generate(
#         **inputs,
#         max_length=max_length,
#         do_sample=True,
#         temperature=0.7
#     )
#     out = tokenizer.decode(outputs[0], skip_special_tokens=True).strip()
#     return out.split("Question:")[-1].strip()

# # --- FastAPI endpoints ---

# @app.post("/generate/general", response_model=GeneralQuestionResponse)
# def api_general(req: TextRequest):
#     q = generate_general_question(req.text)
#     return GeneralQuestionResponse(question=q)

# @app.post("/generate/mcq", response_model=MCQResponse)
# def api_mcq(req: TextRequest):
#     mcq = generate_mcq_question(req.text)
#     return MCQResponse(**mcq)

# @app.post("/generate/truefalse", response_model=TrueFalseResponse)
# def api_truefalse(req: TextRequest):
#     tf = generate_true_false_question(req.text)
#     return TrueFalseResponse(**tf)

# @app.post("/generate/coding", response_model=CodingQuestionResponse)
# def api_coding(req: CodeRequest):
#     q = generate_coding_question(req.code)
#     return CodingQuestionResponse(question=q)

# # To run the server, use the command:
# # uvicorn app:app --host
