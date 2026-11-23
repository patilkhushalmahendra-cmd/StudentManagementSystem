# CampusDesk — Student Management System

A polished C# Windows Forms project built for **.NET Framework 4.8**. It is suitable for a college demonstration and does not require SQL Server or NuGet packages.

## Features
- Dashboard cards: total students, active students, average score
- Add and edit student records
- Delete records with confirmation
- Search by student name, course, email, or ID
- Input validation for name and email
- Export records to CSV for Excel
- XML file persistence; records are saved between runs
- Sample records included for demonstration

## Requirements
- Windows
- Visual Studio 2019 or 2022
- **.NET desktop development** workload
- .NET Framework 4.8 Developer Pack / targeting pack

## Run in Visual Studio
1. Extract the ZIP file.
2. Open `StudentManagementSystem.sln` in Visual Studio.
3. If Visual Studio asks to retarget the framework, select .NET Framework 4.8.
4. Choose **Build > Build Solution**.
5. Press **F5** to run.

## Data storage
The app saves its XML data in:
`%LOCALAPPDATA%\\CampusDesk\\students.xml`

To reset the demo data, close the app and delete that file. The sample data will be recreated the next time the app opens.

## Suggested project presentation
**Title:** CampusDesk — Student Management System  
**Problem statement:** Maintaining student information manually is time-consuming and makes searching, updating, and reporting records difficult.  
**Objective:** Build a desktop application to manage student details in one place.  
**Technology:** C#, Windows Forms, .NET Framework 4.8, XML serialization, CSV export.  
**Modules:** Dashboard, student CRUD, search, validation, persistent storage, CSV export.  
**Future scope:** Add SQL Server database, login roles, attendance, fee tracking, and report generation.

## Demo sequence
1. Show the dashboard and sample records.
2. Search for a student using the search box.
3. Add a new student and save.
4. Select a row, change the details, and save again.
5. Delete a test record.
6. Export CSV and open it in Excel.
7. Close and reopen the app to show data persistence.
