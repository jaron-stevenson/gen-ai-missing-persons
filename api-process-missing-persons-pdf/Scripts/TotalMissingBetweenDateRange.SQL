SELECT COUNT(*)
FROM MissingPersons
WHERE DateReported BETWEEN '2024-03-16' AND '2024-04-01'
  AND CurrentStatus = 'Missing';
