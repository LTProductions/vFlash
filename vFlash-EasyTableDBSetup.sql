ALTER TABLE SubclassData
	ADD class_id nvarchar(255) not null;

ALTER TABLE SubclassData
	ADD CONSTRAINT FK_Subclass_Class_Cascade
	FOREIGN KEY (class_id) REFERENCES ClassData(id) ON DELETE CASCADE;

ALTER TABLE FlashcardData
	ADD fcstack_id nvarchar(255) not null;

ALTER TABLE FlashcardData
	ADD CONSTRAINT FK_Flashcard_FCStack_Cascade
	FOREIGN KEY (fcstack_id) REFERENCES FCStackData(id) ON DELETE CASCADE;

ALTER TABLE FCStackData
	ADD subclass_id nvarchar(255) not null;

ALTER TABLE FCStackData
	ADD CONSTRAINT FK_FCStack_Subclass_Cascade
	FOREIGN KEY (subclass_id) REFERENCES SubclassData(id) ON DELETE CASCADE;

ALTER TABLE ScoreData
	ADD fcdata_id nvarchar(255) not null;

ALTER TABLE ScoreData
	ADD CONSTRAINT FK_ScoreData_Flashcard_Cascade
	FOREIGN KEY (fcdata_id) REFERENCES FlashcardData(id) ON DELETE CASCADE;

ALTER TABLE ScoreData
	ADD sessiondata_id nvarchar(255) not null;

ALTER TABLE ScoreData
	ADD CONSTRAINT FK_ScoreData_Session_Cascade
	FOREIGN KEY (sessiondata_id) REFERENCES StudySessionData(id) ON DELETE CASCADE;

ALTER TABLE StudySessionData
	ADD sessionname_id nvarchar(255) not null;

ALTER TABLE StudySessionData
	ADD CONSTRAINT FK_Session_SessionName_Cascade
	FOREIGN KEY (sessionname_id) REFERENCES StudySessionNames(id) ON DELETE CASCADE;