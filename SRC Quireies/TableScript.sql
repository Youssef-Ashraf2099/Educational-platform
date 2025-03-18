--create database
--use database

CREATE TABLE Learner (
    Learner_ID INT PRIMARY KEY IDENTITY,
    first_name VARCHAR(20),
    last_name VARCHAR(20),
    birthday DATE,
    gender VARCHAR(10) CHECK (gender IN ('Male', 'Female')),
    country VARCHAR(20),
    cultural_background VARCHAR(50),
	password varchar(50),
	email varchar(50)
);
CREATE TABLE Skills(
Learner_ID INT ,
FOREIGN KEY (Learner_ID) REFERENCES Learner(Learner_ID)ON DELETE CASCADE ON UPDATE CASCADE,
skill VARCHAR(50),
primary key(Learner_ID,skill)

);

CREATE TABLE LearningPrefrences(
Learner_ID INT,
FOREIGN KEY (Learner_ID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
prefrences VARCHAR(50),
primary key(Learner_ID,prefrences)

);

CREATE TABLE PersonalProfile(
Learner_ID INT ,
profileID INT,
PreferedContent_type VARCHAR(50),
emotionalState VARCHAR(50),
personality_type VARCHAR(50),
primary key(Learner_ID,profileID),
FOREIGN KEY (Learner_ID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE

);
CREATE TABLE HealthCondition(
Learner_ID INT,
profileID INT ,
FOREIGN KEY (Learner_ID,profileID) REFERENCES PersonalProfile(Learner_ID,profileID) ON DELETE CASCADE ON UPDATE CASCADE,
condition VARCHAR(50),
primary key(Learner_ID,profileID,condition)

);
CREATE TABLE Course (
    Course_ID INT PRIMARY KEY IDENTITY,
    title VARCHAR(100),
    description VARCHAR(255),
    diff_level VARCHAR(8),
    credit_point INT,
   learning_objective VARCHAR(255),
  
   
);


CREATE TABLE CoursePrerequisites(
Course_ID INT,
FOREIGN KEY (Course_ID) REFERENCES Course(Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
prerequisite VARCHAR(250),
primary key (Course_ID,prerequisite)
);
CREATE TABLE Module (
	Module_ID INT IDENTITY,
    Course_ID INT,
	FOREIGN KEY (Course_ID) REFERENCES Course(Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
	title VARCHAR(100),
	difficulty_level VARCHAR(8),
    contentURL VARCHAR(255)	,
	primary key(Module_ID,Course_ID)
);

CREATE TABLE TargetTraits(
Module_ID INT,
Course_ID INT,
FOREIGN KEY (Module_ID,Course_ID) REFERENCES Module(Module_ID,Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
trait VARCHAR(50),
primary key(Module_ID,Course_ID,trait)

);

CREATE TABLE ModuleContent(
Module_ID INT,
Course_ID INT,
FOREIGN KEY (Module_ID,Course_ID) REFERENCES Module(Module_ID,Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
contetntType VARCHAR(50),
primary key(Module_ID,Course_ID,contetntType)

);

CREATE TABLE ContentLibrary(
Lib_ID INT PRIMARY KEY IDENTITY,
Module_ID INT,
Course_ID INT,
FOREIGN KEY (Module_ID,Course_ID) REFERENCES Module(Module_ID,Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
title VARCHAR(100),
description VARCHAR(255),
metaData VARCHAR(255),
type VARCHAR(50),
contentURL VARCHAR(255)
);

CREATE TABLE Course_Enrollment(
Enrollment_ID INT PRIMARY KEY IDENTITY,
Learner_ID INT,
Course_ID INT,
FOREIGN KEY (Learner_ID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (Course_ID) REFERENCES Course(Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
enrollment_date DATE,
completion_date DATE,
status VARCHAR(50) CHECK (status IN ('Completed', 'In Progress', 'Not Started'))
);

CREATE TABLE Assessment (
	Assessment_ID INT PRIMARY KEY IDENTITY,
    Module_ID INT,
    Course_ID INT,
FOREIGN KEY (Module_ID,Course_ID) REFERENCES Module(Module_ID,Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
	type VARCHAR(50),
    totalMarks INT,
    passingMarks INT,
    criteria VARCHAR(255),
    weightage INT,
    description VARCHAR(255),
    title VARCHAR(100)
);

CREATE TABLE TakenAssessment(
Assessment_ID INT,
Learner_ID INT,
FOREIGN KEY (Assessment_ID) REFERENCES Assessment(Assessment_ID) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (Learner_ID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
ScoredPoint INT
PRIMARY KEY (Assessment_ID, Learner_ID)
);

CREATE TABLE Instructor (
	Instructor_ID INT PRIMARY KEY IDENTITY,
	Instructor_name VARCHAR(20),
	latest_qualification VARCHAR(20),
    expertise_area VARCHAR(50),
    email VARCHAR(50),
	password varchar(50)
    );

	    CREATE TABLE LearningPath(        
    Path_ID INT PRIMARY KEY IDENTITY,
    Learner_ID INT,
    profileID INT,
    FOREIGN KEY (Learner_ID,profileID) REFERENCES PersonalProfile(Learner_ID,profileID) ON DELETE CASCADE ON UPDATE CASCADE,
    completion_status VARCHAR(220),
    customContent VARCHAR(255),
    adaptiveRules VARCHAR(255)
    );

    CREATE TABLE pathreview(
    Instructor_ID INT,
    Path_ID INT,
    FOREIGN KEY (Instructor_ID) REFERENCES Instructor(Instructor_ID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Path_ID) REFERENCES LearningPath(Path_ID) ON DELETE CASCADE ON UPDATE CASCADE,
    feedback VARCHAR(255),
	primary key(Instructor_ID,Path_ID)
    );

CREATE TABLE learningActivity(
    Activity_ID INT PRIMARY KEY IDENTITY,
    Course_ID INT,
    Module_ID INT,
    FOREIGN KEY (Module_ID,Course_ID) REFERENCES Module(Module_ID,Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
    activityType VARCHAR(50),
    instruction_details VARCHAR(255),
    maxScore INT
    );

	 CREATE TABLE Emotional_feedback(
    FeedbackID INT PRIMARY KEY IDENTITY,
    LearnerID INT ,
	Activity_ID int,
    timestamp DATETIME,
    emotional_state VARCHAR(20) CHECK (emotional_state in (
            'Admiration', 'Adoration', 'Aesthetic Appreciation', 'Amusement', 
            'Anger', 'Anxiety', 'Awe', 'Awkwardness', 'Boredom', 'Calmness', 
            'Confusion', 'Craving', 'Disgust', 'Empathetic Pain', 'Entrancement', 
            'Excitement', 'Fear', 'Horror', 'Interest', 'Joy', 
            'Nostalgia', 'Relief', 'Romance', 'Sadness', 'Satisfaction', 
            'Surprise'
        ))
        FOREIGN KEY(LearnerID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
		FOREIGN KEY(Activity_ID) REFERENCES learningActivity(Activity_ID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Emotionalfeedback_review (
    FeedbackID INT ,
    InstructorID INT,
    review VARCHAR(500),
    PRIMARY KEY (FeedbackID, InstructorID),
    FOREIGN KEY (FeedbackID) REFERENCES Emotional_feedback(FeedbackID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (InstructorID) REFERENCES Instructor(Instructor_ID) ON DELETE CASCADE ON UPDATE CASCADE
);
    CREATE TABLE Teaches(
    Instructor_ID INT,
    Course_ID INT,
    FOREIGN KEY (Instructor_ID) REFERENCES Instructor(Instructor_ID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Course_ID) REFERENCES Course(Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
	primary key(Instructor_ID,Course_ID)
	);




    CREATE TABLE Notification(
    Notification_ID INT PRIMARY KEY IDENTITY,
    time_stamp datetime default getdate(),
    message VARCHAR(255),
    urgency VARCHAR(50) CHECK (urgency IN ('High', 'Medium', 'Low')),
	readstatus bit
    );
	

    Create TABLE RecivedNotfy(
    Learner_ID INT,
    Notification_ID INT,
    FOREIGN KEY (Learner_ID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (Notification_ID) REFERENCES Notification(Notification_ID) ON DELETE CASCADE ON UPDATE CASCADE,
	primary key(Learner_ID,Notification_ID)

	);
    CREATE TABLE Reward(
    RewardID INT PRIMARY KEY IDENTITY,
    value INT,
    description VARCHAR(200),
    type VARCHAR(50)
);

   

CREATE TABLE Interaction_log (
    LogID INT PRIMARY KEY IDENTITY,
    activity_ID INT,
    LearnerID INT,
    Duration TIME,
    Timestamp DATETIME ,
    action_type VARCHAR(50),
    FOREIGN KEY (activity_ID) REFERENCES learningActivity(Activity_ID) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (LearnerID) REFERENCES Learner( Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE
    );

 CREATE table Quest(
QuestID INT PRIMARY KEY IDENTITY,
difficulty_level int,
criteria VARCHAR(50),
description VARCHAR(200),
title VARCHAR(60)
);


CREATE TABLE QuestReward(
	QuestID INT,
	RewardID INT,
    LearnerID INT,
    PRIMARY KEY (QuestID, RewardID,LearnerID),
    timeEarned datetime default getdate(),
	FOREIGN KEY (QuestID) REFERENCES Quest(QuestID) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (RewardID) REFERENCES Reward(RewardID) ON DELETE CASCADE ON UPDATE CASCADE,
	FOREIGN KEY (LearnerID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE
	
);
CREATE TABLE Skill_Mastery (
    QuestID INT unique ,
    Skill VARCHAR(255),
	primary key(QuestID,Skill),
    FOREIGN KEY (QuestID) REFERENCES Quest(QuestID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Collaborative (
    QuestID INT PRIMARY KEY,
    Deadline DATE,
    Max_Num_Participants INT,
    FOREIGN KEY (QuestID) REFERENCES Quest(QuestID) ON DELETE CASCADE ON UPDATE CASCADE

);
CREATE TABLE LearnerCollaboration(
LearnerId INT,
QuestID INT,
FOREIGN KEY (LearnerId) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (QuestID) REFERENCES Collaborative(QuestID) ON DELETE CASCADE ON UPDATE CASCADE,
completion_status VARCHAR(50) CHECK (completion_status IN ('Completed', 'In Progress', 'Not Started')),
primary key(LearnerId,QuestID)
);

CREATE TABLE LearnerMastery(
LearnerID INT,
QuestID INT,
FOREIGN KEY (LearnerID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (QuestID) REFERENCES Skill_Mastery(QuestID) ON DELETE CASCADE ON UPDATE CASCADE,
completion_status VARCHAR(50) CHECK (completion_status IN ('Completed', 'In Progress', 'Not Started')),
primary key(LearnerID,QuestID)
);

    CREATE TABLE Badge (
    BadgeID int PRIMARY KEY IDENTITY,
    title VARCHAR(50),
    description VARCHAR(200),
    criteria VARCHAR(50),
    points int
);

	Create TABLE Discussion_forum (
	forumID int primary key IDENTITY,
	Course_ID int,
	Module_ID int,
	 FOREIGN KEY (Module_ID,Course_ID) REFERENCES Module(Module_ID,Course_ID) ON DELETE CASCADE ON UPDATE CASCADE,
	 title varchar(50),
	 last_active Datetime,
	 timestamp datetime,
	 description varchar(255)
	);


CREATE TABLE Survey (
    ID INT PRIMARY KEY IDENTITY,         
    Title VARCHAR(255) NOT NULL  

);

CREATE TABLE SurveyQuestions (
    SurveyID INT,                  
    Question VARCHAR(255) NOT NULL,   
    PRIMARY KEY (SurveyID, Question), 
    FOREIGN KEY (SurveyID) REFERENCES Survey(ID)  ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE LearnerDiscussion(
ForumID INT,
LearnerID INT,
FOREIGN KEY (ForumID) REFERENCES Discussion_forum(forumID) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (LearnerID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
post VARCHAR(255),
time time,
primary key(ForumID,LearnerID,post)
);

CREATE TABLE FilledSurvey (
    SurveyID INT,
    Question VARCHAR(255) NOT NULL,
    LearnerID INT,
    Answer TEXT NOT NULL,
    PRIMARY KEY (SurveyID, Question, LearnerID),
    FOREIGN KEY (SurveyID, Question) REFERENCES SurveyQuestions(SurveyID, Question) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (LearnerID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE Achievement (
    AchievementID INT PRIMARY KEY IDENTITY,
    LearnerID INT,
    BadgeID INT,
    Description TEXT,
    DateEarned DATE NOT NULL,
    Type VARCHAR(50) NOT NULL,
    FOREIGN KEY (LearnerID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (BadgeID) REFERENCES Badge(BadgeID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE SkillProgression (
    ID INT PRIMARY KEY IDENTITY,
    proficiency_level INT NOT NULL,
    LearnerID INT ,
    skill_name VARCHAR(50),
    timestamp datetime NOT NULL,
    FOREIGN KEY (LearnerID, skill_name) REFERENCES Skills(Learner_ID, skill) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Leaderboard (
    BoardID INT PRIMARY KEY IDENTITY,      
    season VARCHAR(20) NOT NULL

);
CREATE TABLE Ranking (
    BoardID INT,                    
    LearnerID INT,                    
    CourseID INT,                     
    rank INT NOT NULL,                
    total_points INT NOT NULL,         
    PRIMARY KEY (BoardID, LearnerID),
    FOREIGN KEY (BoardID) REFERENCES Leaderboard(BoardID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (LearnerID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (CourseID) REFERENCES Course(Course_ID) ON DELETE CASCADE ON UPDATE CASCADE
);
CREATE TABLE Learning_goal (
    ID INT PRIMARY KEY IDENTITY,             
    status VARCHAR(20) NOT NULL,    
    deadline DATE NOT NULL,        
    description TEXT NOT NULL       
);
CREATE TABLE LearnersGoals (
    GoalID INT,                    
    LearnerID INT,                
    PRIMARY KEY (GoalID, LearnerID) ,
    FOREIGN KEY (GoalID) REFERENCES Learning_goal(ID) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (LearnerID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE LearnerQuest(
LearnerId INT,
QuestId INT,
AssignedDate Datetime,
status varchar(20),
Primary key(QuestID,LearnerID),

FOREIGN KEY (LearnerID) REFERENCES Learner(Learner_ID) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY (QuestID) REFERENCES Quest(QuestID) ON DELETE CASCADE ON UPDATE CASCADE
);
