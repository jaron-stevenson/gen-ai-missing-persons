-- Lets retreive a count of missing persons by each age group.

SELECT Age, COUNT(*) AS NumberOfMissingPersons
FROM MissingPersons
WHERE CurrentStatus = 'Missing'
GROUP BY Age
ORDER BY Age;
