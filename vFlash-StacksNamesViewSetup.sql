CREATE VIEW [StacksNamesView] AS
	SELECT ClassData.name as ClassName, ClassData.id as id,
	ClassData.createdAt as createdAt, ClassData.updatedAt as updatedAt,
	ClassData.version as version, ClassData.Deleted as deleted,
	SubclassData.name as SubclassName, FCStackData.name as StackName,
	FCStackData.id as StackID
	FROM ((ClassData
		INNER JOIN SubclassData ON ClassData.id = SubclassData.class_id)
		INNER JOIN FCStackData ON SubclassData.id = FCStackData.subclass_id);