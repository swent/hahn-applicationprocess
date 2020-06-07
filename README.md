# Hahn Applicant Management Platform

This is a small demo of an asp.net core 3.1 webservice supported by entity frameworks in-memory database and hosting an aurelia/boostrap frontend written in typescript.

### To develop:
1. Go into `Hahn.ApplicationProcess.May2020.Web/Frontend` folder and restore packages: `npm install`
2. Build the frontend `npm run build:dev` (one time) or watch it `npm run watch`
3. Make sure you have a valid dotnet development certificate `dotnet dev-certs https --trust`
4. If you don't have a valid certificate disable `https`/`hsts` in `Hahn.ApplicationProcess.May2020.Web/Startup.cs:78`
5. Run the main project from your IDE or use `dotnet build Hahn.ApplicationProcess.May2020.Web/Hahn.ApplicationProcess.May2020.Web.csproj`
6. Open browser on `https://localhost:5001` (or `http://localhost:5000` when https has been disabled)

### To publish (currently only supported in windows environment):
1. Run `dotnet publish Hahn.ApplicationProcess.May2020.Web/Hahn.ApplicationProcess.May2020.Web.csproj -r win-x64 --self-contained false -o YOUR_OUTPUT_PATH`
2. Change to your output directory and run `dotnet Hahn.ApplicationProcess.May2020.Web.dll`
6. Open browser on `https://localhost:5001` (or `http://localhost:5000` when https has been disabled)

**Publishing from your IDE might very well fail since there are lot's of problems in sdk version resolving for .net core >= 3.0.**

While the backend part was pretty straight forward, there have been many struggles on my way learning the aurelia framework. As of writing this I am sure the frontend code can be improved on.

All included assets have ben sourced from free to use repositories.

If you encounter any problems, please let me know.
