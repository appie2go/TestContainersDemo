# Unit testing a stored procedure with Entity Framework and TestContainers

This repo contains an API which uses a stored procedure to collect data from a database. The stored procedure is executed with SQL Server. Typically, this is a developers worst nightmare.

Since in some cases you need to test such scenario's, in the test-project you can find a way to test it with TestContainers.
