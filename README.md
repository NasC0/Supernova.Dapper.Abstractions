# Supernova Dapper Abstractions

## Description
An abstraction over StackOveflow's Dapper ORM for .NET, which provides additional functionality and ease-of-access.

1. Introduction
2. Usage

## 1. Introduction
An abstraction over StackOveflow's Dapper ORM for .NET, which provides additional functionality and ease-of-access.

Provides abstract Read/Write repositories, which contain basic logic for convention based CRUD operations on top of the Dapper ORM framework.

The abstract repositories support Select, Insert (+ bulk), Update (+bulk), Delete (+bulk) functionality.

SQL language parsers are introduced for a range of database server.

## 2. Usage
### 2.1 Custom attributes
Provides attributes to decorate entity properties with names different from their Database column counterparts.
Should be used when your entity property has a different name than the column name in the database.

They accept Name(string) as a parameter.

**ColumnAttribute** - Maps your entity property to a column name in the table - use when property has a different name from the column in the database table.

**PrimaryKeyAttribute** - Marks your entity property as a primary key (should be set always). Accepts a Name parameter in case your id property has a different name from the id column in the database table.

**TableNameAttribute** - Maps your entity to the database table.

## 2.2 DapperStartupMapping static class
Accepts namespaces, which contain database entity models.
Registers Dapper typemaps - typemaps are the way Dapper resolves Column-to-Property mapping in entities.
Collects properties decorated with the custom derivatives of the BaseAttribute and scans their Name parameter for a value, mapping the database table column name to the property/attribute name.


## 3. Database (Relational) servers support
Current version of the Supernova.Dapper package supports the following servers:
- SQL Server
