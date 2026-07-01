// =========================================================================================
// 1. THE FOUNDATION: WebApplication.CreateBuilder
// =========================================================================================
// DEFINITION: 
// This is the absolute starting point of your .NET web app. Before this line executes, 
// your application is just text in a file. After it executes, the foundational 
// infrastructure is alive and ready to be customized.
//
// THE RESTAURANT ANALOGY: 
// You are opening a high-end restaurant. This line is the exact moment you hire the 
// General Manager and hand them the keys to the empty building. The Manager immediately 
// steps inside and performs 3 critical setup tasks automatically.
//
// HOW IT WORKS (The 3 Major Automated Tasks):
//
//   1. SETS UP KESTREL (The Front Doors & Host Stand):
//      - Kestrel is the built-in web server. 
//      - How it works: Your C# code is trapped in the kitchen and cannot talk to the 
//        internet. Kestrel binds to your computer's network ports to listen for traffic.
//      - Analogy: The Manager unlocks the front doors and builds a Host Stand. Without 
//        this, hungry customers (web requests) would just hit a brick wall.
//
//   2. READS CONFIGURATIONS (The Manager's Master Ledger):
//      - It automatically searches for files like `appsettings.json` and loads them 
//        into memory.
//      - How it works: You put database passwords and environment settings here instead 
//        of hard-coding them into your C# files for the whole world to see.
//      - Analogy: The Manager opens the Master Ledger containing the safe combination, 
//        supplier phone numbers, and the rulebook for tonight's service.
//
//   3. CREATES THE D.I. CONTAINER (The Empty Kitchen Pantry):
//      - It creates `builder.Services`, your Dependency Injection container.
//      - How it works: This establishes the central storage area for all your app's tools.
//      - Analogy: The Manager builds a massive walk-in Kitchen Pantry. Because this line 
//        just ran, the pantry is COMPLETELY EMPTY. In the next lines of code, you will 
//        hire chefs (Controllers) and stock the shelves.
//
// WHAT IS "(args)"? (The VIP Instruction)
//   - Definition: 'args' captures any text commands typed into your terminal at the 
//     exact moment the app is launched (e.g., `dotnet run --environment Production`).
//   - Analogy: As you hand the keys to the Manager, you whisper a special instruction: 
//     "Make sure we only serve the Production menu tonight." The Manager hears this and 
//     adjusts the restaurant setup instantly without you changing any C# code.
// -----------------------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);


// =========================================================================================
// 2. HIRING THE CHEFS: builder.Services.AddControllers()
// =========================================================================================
// DEFINITION: 
// A "Controller" is a C# class responsible for actually handling the customer's 
// internet requests (like GET, POST, PUT, DELETE), processing the logic, and returning 
// the correct data (usually as JSON). 
//
// THE RESTAURANT ANALOGY: 
// In step 1, the General Manager built the Kitchen Pantry (builder.Services), but it 
// was completely empty. This line is the Manager actively HIRING THE CHEFS and putting 
// them on the official payroll. 
// 
// HOW IT WORKS BEHIND THE SCENES:
//   - The Scout: When this line runs, ASP.NET acts like a talent scout. It rapidly 
//     scans every single file in your entire C# project.
//   - The Nametag: It specifically looks for any class wearing the `[ApiController]` 
//     nametag (the C# attribute). 
//   - The Roster: When it finds one, it registers that class into the DI Container 
//     (the Kitchen Pantry). Now, the restaurant officially knows this chef works here.
//
// WHY IT IS NECESSARY:
//   If you delete this line, your app will still start, and Kestrel will still listen 
//   at the front door. But when a customer places an order, the waiter will walk into 
//   the kitchen, realize there are zero chefs on the payroll, and return to the table 
//   with a "404 Not Found" error. 
//
// EXAMPLES IN YOUR KITCHEN:
//   - You might have a `CatsController`. This is your Feline Specialist Chef. When a 
//     request comes in for pet supplies or nutritional data, this chef handles it.
//   - You might have a `VehiclesController`. This is your Automotive Chef. When a 
//     request comes in to fetch the specs of a VW Virtus GT or a Classic 350, this 
//     chef knows exactly how to query the database and serve that specific data.
// -----------------------------------------------------------------------------------------
builder.Services.AddControllers();


// =========================================================================================
// 3. DESIGNING THE INTERACTIVE MENU: Swagger (API Documentation)
// =========================================================================================
// DEFINITION: 
// OpenAPI (often called Swagger) is a set of tools that automatically reads your C# code 
// and generates a beautiful, interactive webpage. This webpage acts as a testing ground 
// where you can click buttons to test your API without needing to write a single line 
// of frontend code (like React or HTML).
//
// THE RESTAURANT ANALOGY: 
// The General Manager has set up the building (Step 1) and hired the chefs (Step 2). 
// But right now, if a customer walks in, they have no idea what food is available. 
// These two lines of code are the Manager creating a glossy, interactive Menu for the 
// dining room so customers know exactly what they can order.
//
// HOW IT WORKS BEHIND THE SCENES (Line by Line):
//
//   - AddEndpointsApiExplorer() (The Menu Writer): 
//     The Manager hires a scout with a clipboard. This scout walks into the kitchen, 
//     interviews every Chef (Controller) you just hired, and writes down every single 
//     dish they know how to make. Every `[HttpPost]` (like a request to add new pet 
//     supplies to the database) and maps them out.
//
//   - AddSwaggerGen() (The Graphic Designer & Printer): 
//     The Manager takes the scout's handwritten clipboard notes and hands them to a 
//     graphic designer. This tool takes the raw list of endpoints and generates the 
//     actual visual, interactive HTML webpage (the glossy menu). 
//
// WHY IT IS NECESSARY:
//   Without this, you would have to manually build an entire frontend website just to 
//   test if your backend code is working. With Swagger, you press "Play" in Visual 
//   Studio, and a webpage instantly pops up listing all your URLs, allowing you to 
//   type in test data and hit "Execute" to see if the Chefs cook the right data.
// -----------------------------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// =========================================================================================
// 4. NETWORK SECURITY: Registering CORS (Cross-Origin Resource Sharing)
// =========================================================================================
// DEFINITION & THE PROBLEM: 
// CORS is a strict security rule enforced by the user's WEB BROWSER (like Chrome), not 
// by this C# backend. 
//
// By default, browsers have a "Same-Origin Policy". This means if a Frontend website is 
// running on Port 3000, and it tries to ask for database data from this C# Backend running 
// on Port 5000, Chrome will panic and BLOCK the data because the ports don't match.
//
// WHAT HAPPENS IF WE DON'T USE THIS CODE?
// If a separate frontend website tries to connect to this API, this C# code will successfully 
// get the data from the database and send it. But the user's browser will throw it in the 
// trash and show a massive red "CORS Error" on the screen.
//
// THE SOLUTION (What this code does):
// This code explicitly generates a "Permission Slip" named "AllowAll" that tells the 
// user's web browser it is safe to accept the data from this backend.
//
// HOW IT WORKS BEHIND THE SCENES:
//
//   - options.AddPolicy("AllowAll", ...)
//     Creates a specific security policy and names it "AllowAll". 
//
//   - policy.AllowAnyOrigin()
//     "Origin" = The address making the request. 
//     This tells the browser: "My backend allows ANY frontend website, no matter what 
//     port or domain it is running on (Port 3000, google.com, etc.), to read my data."
//
//   - policy.AllowAnyMethod()
//     "Method" = The HTTP action (GET, POST, PUT, DELETE).
//     This tells the browser: "Allow the frontend to use all of these commands. Do not 
//     block a request just because it is a POST or DELETE request."
//
//   - policy.AllowAnyHeader()
//     "Headers" = Hidden metadata attached to a request (like JSON format or login tokens).
//     This tells the browser: "Accept any extra hidden metadata the frontend sends."
//
// CURRENT CONTEXT NOTE: 
// Right now, you are only using Swagger to test. Since Swagger runs on the exact same 
// port as this backend, you technically don't need this yet. But the absolute second 
// you build a real frontend on a different port, this code becomes mandatory.
// -----------------------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// =========================================================================================
// PHASE 2: THE HTTP MIDDLEWARE PIPELINE
// =========================================================================================
// DEFINITION - Middleware: 
// Software components arranged in a chain (a pipeline). When an HTTP request comes in from 
// the internet, it passes through this chain sequentially. Each piece of middleware can:
//   1. Inspect the request.
//   2. Modify the request.
//   3. Reject the request completely.
//   4. Pass it to the next piece of middleware in the chain.

// -----------------------------------------------------------------------------------------
// 5. Building the App
// -----------------------------------------------------------------------------------------
// DEFINITION - Build(): 
// This command permanently locks the DI Container (`builder.Services`). You cannot add any 
// more tools to the warehouse after this line. It returns the `app` object, which represents 
// the actual live web server.
var app = builder.Build();


// -----------------------------------------------------------------------------------------
// 6. Activating Swagger Middleware
// -----------------------------------------------------------------------------------------
// HOW IT WORKS: We registered Swagger in Phase 1, but these lines actually insert it into 
// the HTTP pipeline. If a user visits the "/swagger" URL, this middleware intercepts the 
// request and returns the Swagger testing webpage.
app.UseSwagger();
app.UseSwaggerUI();


// -----------------------------------------------------------------------------------------
// 7. Activating CORS Middleware
// -----------------------------------------------------------------------------------------
// HOW IT WORKS: This inserts the CORS security guard into the pipeline. We hand it the 
// "AllowAll" rulebook we wrote earlier. Now, when a browser asks if it is allowed to connect, 
// this middleware intercepts the question and replies: "Yes, you are allowed!"
app.UseCors("AllowAll");


// -----------------------------------------------------------------------------------------
// 8. Activating HTTPS Redirection Middleware
// -----------------------------------------------------------------------------------------
// DEFINITION - HTTPS (Hypertext Transfer Protocol Secure): 
// An encrypted version of the internet protocol. It scrambles data so hackers cannot read it.
//
// HOW IT WORKS: If a user accidentally types "http://" (unsecure) in their browser, this 
// middleware intercepts the request and forces their browser to reload using "https://".
app.UseHttpsRedirection();
    
// -----------------------------------------------------------------------------------------
// 10. Endpoint Routing
// -----------------------------------------------------------------------------------------
// DEFINITION - Routing: 
// The process of matching a URL typed into the browser to a specific C# method in your code.
//
// HOW IT WORKS: When a request for "GET /api/cats" reaches this point in the pipeline, 
// `MapControllers()` looks at its internal map, realizes that URL belongs to your 
// `CatsController`, and successfully delivers the network request to your C# method.
app.MapControllers();


// -----------------------------------------------------------------------------------------
// 11. Run the Server
// -----------------------------------------------------------------------------------------
// DEFINITION - Run: 
// A blocking method that starts the application's message loop. 
//
// HOW IT WORKS: This line prevents the application from closing. It binds the app to your 
// computer's network ports and sits in an infinite loop, constantly listening for new 
// internet traffic until you press the Stop button in Visual Studio.
app.Run();