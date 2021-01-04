--Part 1
SELECT * FROM techjobs.jobs;
--Part 2
SELECT name
	FROM techjobs.employers
	WHERE location = "St. Louis City";
--Part 3
SELECT DISTINCT Name, Description
	FROM techjobs.skills
	INNER JOIN techjobs.jobskills ON techjobs.skills.Id = techjobs.jobskills.SkillId
	ORDER BY Name ASC;
