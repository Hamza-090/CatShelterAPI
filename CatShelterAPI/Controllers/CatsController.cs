
// =====================================================================
// FILE: CatsController.cs
// =====================================================================

// WHAT ARE THESE "using" LINES?
// These are like "import tools from a toolbox"
// Without these lines, C# doesn't know what SqlConnection, 
// IConfiguration, etc. are. They don't exist unless you import them.
// Think of it like USE CatShelterDB in SSMS — you tell it where to look.
using CatShelterAPI.Models;         // gives us: our Cat class from Cat.cs
using Microsoft.AspNetCore.Mvc;     // gives us: ControllerBase, IActionResult, Ok(), [ApiController], [HttpGet] etc.
using Microsoft.Data.SqlClient;     // gives us: SqlConnection, SqlCommand, SqlDataReader

// WHAT IS A NAMESPACE?
// Just an address/folder for your code.
// Like how CatShelterDB is the database name and Cats is the table name.
// CatShelterAPI.Controllers is just the address of this file.
// Without it, two files with same class name would crash into each other.
namespace CatShelterAPI.Controllers
{
    // ==========================================================
    // 1. [ApiController] — THE AUTOMATIC BOUNCER
    // ==========================================================
    // Imagine you run a club, and the rule is: "You must be a number to enter."
    //
    // WITHOUT this attribute:
    // If the word "Meow" tries to enter, YOU have to write C# code to stop it.
    // You have to write: "if data is not a number, send an error..."
    //
    // WITH this attribute:
    // Microsoft placed a giant, automatic bouncer at the front door.
    // If "Meow" tries to enter where a number belongs, the bouncer instantly 
    // rejects it and sends a "400 Bad Request" error back to the user. 
    // You don't have to write a single line of checking code. It just works.

    // ==========================================================
    // 2. [Route("api/cats")] — THE DEPARTMENT SIGN
    // ==========================================================
    // Imagine walking into a massive hospital. 
    // If you just say "I need a doctor," the receptionist won't know where to send you.
    // You need to go to the "Cardiology Department" or the "Neurology Department".
    //
    // Your API is the hospital. 
    // [Route("api/cats")] is the neon sign above the door that says "CAT DEPARTMENT".
    // 
    // When the browser visits the URL ".../api/cats", ASP.NET walks down the hallway, 
    // reads all the neon signs, finds the one that matches perfectly, and opens the door.
    // Without this sign, ASP.NET has no idea which controller to use!

    [ApiController]
    [Route("api/cats")]



    // WHAT IS ControllerBase?
    // ControllerBase is a massive, pre-written C# class created by Microsoft. It contains 
    // thousands of lines of complicated internet networking code.
    //
    // WHY DO WE INHERIT IT? (The Restaurant Analogy)
    // ❌ IF WE DID NOT INHERIT IT: We would be standing in an empty dirt lot. We would have to 
    //    lay the bricks, build the ovens, and install the internet plumbing ourselves.
    // ✅ BECAUSE WE INHERIT IT: Microsoft hands us the keys to a fully built restaurant. 
    //    The internet plumbing already works. We just walk in and start cooking.
    //
    // THE BIG CONTRADICTION: WHY INHERIT A CLASS INSTEAD OF AN INTERFACE?
    // - We use Interfaces when we want to ASK FOR A TOOL (like IConfiguration). 
    //   An interface is just a blueprint (rules).
    // - We use Classes when we want to INHERIT A FOUNDATION. If Microsoft gave us an interface 
    //   here, they would just give us a blueprint that says "You must build a web response system." 
    //   We can't live in a blueprint! We inherit a CLASS so we get the ACTUAL compiled code for free.

    // ------------------------------------------------------------------------------------------
    // PART 2: THE RESPONSE SYSTEM (Ok vs Console.WriteLine)
    // ------------------------------------------------------------------------------------------
    // 📓 Console.WriteLine() is like a private diary. It only prints text to the black terminal 
    //    screen on your specific physical computer. Nobody on the internet can see your monitor.
    //
    // 📦 Ok() is like a professional FedEx Shipping Department. 
    //    When you type Ok(cats), you hand your C# data to FedEx. It boxes it up, translates it, 
    //    stamps it with a "200 Success" label, and SHIPS it across the internet to the browser.

    // ------------------------------------------------------------------------------------------
    // PART 3: DE-JARGONING THE INTERNET (The Post Office Rules)
    // ------------------------------------------------------------------------------------------
    // If we didn't have ControllerBase, we would have to write the raw networking code manually. 
    // Here is what that jargon actually means in Post Office terms:
    //
    // 1. "NETWORK PROTOCOLS" = The Post Office Rules. 
    //    You can't throw a naked toy cat in a mailbox. It must be in a box, with an address. 
    //    ControllerBase acts as your diplomat, perfectly following all the strict computer 
    //    conversation rules (HTTP) so the connection doesn't crash.
    //
    // 2. "HEADERS" = The Shipping Label.
    //    Imagine getting a blank cardboard box with no warning labels. You wouldn't know how to 
    //    open it! Headers are the invisible stickers on internet packages (e.g., "Content-Type: JSON"). 
    //    ControllerBase prints and tapes this shipping label to your box automatically.
    //
    // 3. "STATUS CODES" = The Tracking Stamp.
    //    A 3-digit number telling the browser what happened:
    //    - 200 (Ok)         = "Delivered successfully!"
    //    - 400 (BadRequest) = "Return to Sender: You forgot the zip code! (Bad Data)"
    //    - 404 (NotFound)   = "Return to Sender: Address does not exist! (Cat not found)"
    //    - 500 (StatusCode) = "Internal Server Error (Something broke inside the building)"
    //
    // 4. "SERIALIZATION" = The Translator.
    //    Your server speaks C#. The internet ONLY speaks JSON. ControllerBase acts like an 
    //    automatic Google Translate, instantly turning your C# List into JSON text.
    public class CatsController : ControllerBase
    {
        // =====================================================================
        // WHAT IS THIS VARIABLE?
        // =====================================================================
        // A private variable to store the connection string.
        // private = only THIS class can use it. Nobody outside can access it.
        // readonly = once set in the constructor, it can NEVER be changed.
        //            This is a safety measure — connection string should never change.
        // string = data type, same string you always use
        // _connectionString = the name (underscore prefix is a convention for private variables)
        private readonly string _connectionString;

        // =====================================================================
        // THE CONSTRUCTOR
        // =====================================================================
        // A constructor runs automatically when an object of this class is created.
        // In a normal C# program YOU create the object: new CatsController()
        // In ASP.NET, YOU never create the controller object.
        // ASP.NET creates it automatically every time a browser request comes in.
        // Example: browser visits /api/cats → ASP.NET creates CatsController → runs GetAllCats()
        //
        // -----------------------------------------------------------------------
        // WHAT IS IConfiguration?
        // -----------------------------------------------------------------------
        // IConfiguration is an INTERFACE made by Microsoft.
        // You already know interfaces from Case 44 — they only contain declarations, no code.
        // IConfiguration contains the declaration of GetConnectionString() and other methods.
        //
        // Microsoft also made a CLASS called ConfigurationRoot that IMPLEMENTS IConfiguration.
        // ConfigurationRoot contains the ACTUAL CODE of GetConnectionString() and other methods.
        // This class reads appsettings.json and finds the values inside it.
        // You never see ConfigurationRoot — Microsoft hid it — but it exists and does the work.
        //
        // -----------------------------------------------------------------------
        // WHAT IS "IConfiguration configuration" IN THE PARAMETER?
        // -----------------------------------------------------------------------
        // When you write:
        //     IConfiguration configuration
        // It is actually this behind the scenes:
        //     IConfiguration configuration = new ConfigurationRoot(...)
        //
        // The variable "configuration" is of TYPE IConfiguration (the interface).
        // But the ACTUAL OBJECT inside it is ConfigurationRoot (the class that implements it).
        // This is the same as writing:
        //     IAdoptable animal = new Cat()   ← from your Case 44 practice
        //
        // We use the interface type IConfiguration instead of ConfigurationRoot because:
        // We only care that it HAS GetConnectionString() — which is guaranteed by the interface.
        // We don't care about all the other things inside ConfigurationRoot.
        //
        // -----------------------------------------------------------------------
        // BUT WHO PASSES THIS PARAMETER? YOU NEVER CALLED THIS CONSTRUCTOR!
        // -----------------------------------------------------------------------
        // This is called DEPENDENCY INJECTION. Here is exactly what happens:
        //
        // STEP 1: App starts → Program.cs runs first
        // STEP 2: ASP.NET reads appsettings.json
        //         Creates a ConfigurationRoot object from it
        //         Stores it internally saying "I have IConfiguration ready"
        // STEP 3: Browser visits /api/cats
        // STEP 4: ASP.NET needs to run CatsController
        //         Looks at constructor — sees it needs IConfiguration
        //         ASP.NET already has ConfigurationRoot (which implements IConfiguration)
        //         ASP.NET automatically passes it as the "configuration" parameter
        //         YOU never pass it — ASP.NET does it for you
        // STEP 5: Constructor runs with that configuration object
        //         Calls configuration.GetConnectionString("CatShelterDB")
        //         This goes into appsettings.json → finds "CatShelterDB" → returns the value
        //         That value gets stored in _connectionString
        // STEP 6: Now every method in this controller can use _connectionString
        //         to connect to SQL Server
        //
        // =====================================================================
        // 1. WHY USE IConfiguration (Interface) INSTEAD OF ConfigurationRoot (Class)?
        // =====================================================================
        // Think of an Interface (IConfiguration) as a wall socket.
        // Think of a Class (ConfigurationRoot) as the actual power plant (coal, solar, etc).
        //
        // ❌ BAD (Using the Class): 
        // public CatsController(ConfigurationRoot config)
        // This is like hardwiring your laptop directly to a coal power plant. 
        // If Microsoft deletes that class in the future, your code breaks everywhere!
        //
        // ✅ GOOD (Using the Interface): 
        // public CatsController(IConfiguration config)
        // This is like plugging into the wall socket. You are saying: 
        // "I don't care where the power comes from, just give me my GetConnectionString() method!"
        // If Microsoft changes the background code tomorrow, your code keeps working.
        //
        // This is called "Loose Coupling" — the #1 rule of good software architecture.

        // =====================================================================
        // 2. WHAT IS DEPENDENCY INJECTION (DI)?
        // =====================================================================
        // DI means your class receives the tools it needs from the OUTSIDE, 
        // rather than building them itself.
        //
        // Imagine you are building a house (CatsController) and need a hammer (IConfiguration).
        //
        // ❌ WITHOUT DI (The Hard Way):
        // You stop building, go to the forest, chop wood, mine iron, and make the hammer yourself.
        // Code: IConfiguration config = new ConfigurationRoot();
        //
        // ✅ WITH DI (The Smart Way):
        // You just stand at the building site with your hand out. The manager (ASP.NET) 
        // walks by and hands you the hammer automatically. You just use it.
        // Code: public CatsController(IConfiguration config)
        //
        // We are "injecting" the dependency (the tool) into the class from the outside!
        // -----------------------------------------------------------------------
        // WHY NOT JUST HARDCODE THE CONNECTION STRING?
        // -----------------------------------------------------------------------
        // You COULD write:
        //     private string _connectionString = "Server=DESKTOP-HR74PFV;Database=...";
        // And it would work.
        // But appsettings.json is ONE central place to change it.
        // If your server changes, you change it in ONE file — not in every C# file.
        // -----------------------------------------------------------------------
        public CatsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CatShelterDB");
        }

        // ==========================================================
        // 1. WHAT IS [HttpGet]? (THE SPECIFIC DOOR)
        // ==========================================================
        // If [Route("api/cats")] is the neon sign for the "Cat Department", 
        // [HttpGet] is the specific DOOR inside that department.
        //
        // Inside the Cat Department, you have different desks:
        // 🚪 [HttpGet]    = The "Information Desk" (Give me data / READ)
        // 🚪 [HttpPost]   = The "Admissions Desk" (Here is a new cat / ADD)
        // 🚪 [HttpPut]    = The "Update Desk" (Change this cat's details / UPDATE)
        // 🚪 [HttpDelete] = The "Discharge Desk" (Remove this cat / DELETE)
        //
        // When a browser visits the Cat Department and says "I want to GET data", 
        // ASP.NET points them exactly to the [HttpGet] door.

        // ==========================================================
        // 2. HOW DOES SWAGGER KNOW ABOUT IT? (THE AUTOMATIC MAP)
        // ==========================================================
        // Swagger is like the automated Directory Board in the hospital lobby.
        // 
        // When you press F5 to start your app, Swagger quickly runs through your 
        // entire building (your code). It looks at all your [Route] neon signs and 
        // all your [HttpGet] doors, and automatically draws a clickable map on 
        // your screen. You never have to write code to teach Swagger where things are!

        // ==========================================================
        // 3. HOW DOES IT WORK IN THE REAL WORLD? (THE COURIER)
        // ==========================================================
        // Swagger is just a testing tool for YOU, the developer. Real users will 
        // never see Swagger. They will use your actual website or mobile app.
        //
        // When a real user clicks "View All Cats" on their iPhone, the app acts 
        // like a courier. It drives across the internet to 'https://yourwebsite.com/api/cats', 
        // walks straight up to the [HttpGet] door, grabs the JSON box of cats, 
        // and drives back to show them beautifully on the phone screen!
        // ==========================================================
        [HttpGet]

        // =====================================================================
        // WHAT IS IActionResult?
        // =====================================================================
        // IActionResult is an INTERFACE (like IAdoptable from your C# practice).
        // It represents "any kind of web response".
        //
        // WHY USE IActionResult INSTEAD OF string OR int?
        // Because sometimes you return Ok(cats) — that's a 200 response with data.
        // Sometimes you return StatusCode(500) — that's a 500 error response.
        // Sometimes you return NotFound() — that's a 404 response.
        // All of these are DIFFERENT types but they all implement IActionResult.
        // So IActionResult covers ALL of them.
        //
        // Think of it like this:
        // IActionResult = "I will return SOME kind of web response"
        // Ok()          = one specific type of web response (200 success)
        // StatusCode()  = another specific type of web response (any code)
        //
        // WHAT DOES "WEB RESPONSE" MEAN?
        // When browser asks for cats, it waits.
        // Your method runs, gets the data, then RESPONDS to the browser.
        // That response includes:
        // 1. A status code (200 = OK, 404 = not found, 500 = server error)
        // 2. The actual data (the list of cats as JSON)
        // IActionResult is the C# way of packaging both together.
        public IActionResult GetAllCats()
        {
            // =====================================================================
            // WHAT IS List<Cat>?
            // =====================================================================
            // List<Cat> is a list where EVERY item inside must be of type Cat.
            // Cat is not a primitive type like int or string.
            // Cat is YOUR CLASS from Cat.cs — it has 7 properties.
            // So this list holds Cat OBJECTS — each object has 7 properties.
            //
            // WHY NOT JUST USE List<string>?
            // Because one cat has 7 pieces of data (ID, Name, Breed, Age, Color etc.)
            // A string can only hold ONE piece of text.
            // A Cat object holds ALL 7 pieces together for one cat.
            //
            // This list starts EMPTY. We fill it below from the database.
            List<Cat> cats = new List<Cat>();

            try
            {
                // =====================================================================
                // WHY DO WE OPEN A NEW CONNECTION EVERY TIME?
                // =====================================================================
                // You asked: "the project is linked to the database, 
                // why not connect once and reuse it?"
                //
                // Great question. The answer is CONNECTION POOLING.
                // .NET actually does NOT open a new physical connection every time.
                // Behind the scenes, it maintains a POOL of open connections.
                // When you write "new SqlConnection()" and "con.Open()", 
                // .NET gives you a connection from the pool (already open).
                // When the using block ends, it returns it to the pool.
                // =====================================================================
                // 🌟 ADVANCED POOLING: SIZES, "NEW", AND "OPEN" 🌟
                // =====================================================================

                // ---------------------------------------------------------------------
                // 1. IS THE POOL SIZE PREDEFINED? 
                // ---------------------------------------------------------------------
                // YES. By default, Microsoft sets the maximum SQL Connection Pool to 100.
                // You can actually change this! In your appsettings.json connection string, 
                // you could write: "Server=...; Max Pool Size=500;" 
                // But 100 is the default because it is the "Goldilocks" number for most apps.

                // ---------------------------------------------------------------------
                // 2. IF COMPUTERS ARE FAST, AREN'T 10 CONNECTIONS ENOUGH?
                // ---------------------------------------------------------------------
                // You are 100% correct! For a small or medium website, a pool of 10 is 
                // absolutely enough. 
                //
                // But imagine your Cat Shelter goes viral on TikTok. Suddenly, 5,000 people 
                // visit your site at the exact same second. 
                // - If a database search takes a little longer (say, 0.5 seconds because 
                //   it has to search 10,000 photos), the connections stay checked out longer.
                // - If your pool only has 10 phones, a massive traffic jam forms instantly.
                // - Microsoft defaults to 100 just to give you a massive safety buffer 
                //   against sudden traffic spikes.

                // ---------------------------------------------------------------------
                // 3. THE SECRET: WHAT DO "new" AND "Open()" ACTUALLY DO?
                // ---------------------------------------------------------------------
                // This is the exact moment the magic happens. 
                //
                // 👉 WHAT `new SqlConnection()` DOES:
                // It just builds the plastic telephone. It looks at your appsettings.json, 
                // paints the phone the right color, and programs the speed dial. 
                // BUT THE PHONE IS NOT PLUGGED IN YET. It is disconnected.
                // Look at the exact code you wrote inside your method:
                //    using SqlConnection con = new SqlConnection(_connectionString);
                //
                // It knows EXACTLY where to connect because you handed it the _connectionString! 
                // 
                // The _connectionString (which you got from appsettings.json via the wall socket)
                // is the literal "Speed Dial Number" for your kitchen.
                //
                // When you say "new SqlConnection(_connectionString)", you are telling C#:
                // "Grab a database phone from the Pool, and program it to call THIS exact 
                // SQL Server, using THIS exact username and password
                //
                // 👉 WHAT `con.Open()` DOES:
                // THIS is the moment you grab the physical line from the Pool!
                // When you call Open(), ASP.NET says: "Okay, they are ready to talk." 
                // It grabs one of the 100 live, running connections from the Pool, 
                // plugs your plastic phone into it, and establishes the live link to the kitchen.
                //
                // 👉 WHAT `using` (Close) DOES:
                // It unplugs the plastic phone, throws it in the trash, and leaves the 
                // live connection line running in the Pool for the next user to plug into.
                // =====================================================================
                using SqlConnection con = new SqlConnection(_connectionString);
                con.Open(); // Actually opens the connection to SQL Server

                /// =====================================================================
                // 🌟 THE MASTER GUIDE TO: SqlCommand (The Order Ticket) 🌟
                // =====================================================================

                // ---------------------------------------------------------------------
                // 1. WHAT IS SqlCommand? (THE ORDER TICKET)
                // ---------------------------------------------------------------------
                // If SqlConnection is the physical telephone line to the Kitchen...
                // SqlCommand is the EXACT ORDER TICKET you are reading over the phone.
                // 
                // It represents ONE specific operation you want the database to do.
                // When you write: new SqlCommand("sp_GetAllCats", con);
                // You are telling C#: "I want to place an order for 'sp_GetAllCats'."

                // ---------------------------------------------------------------------
                // 2. WHY DOES IT NEED THE "con" PARAMETER? (WHICH PHONE?)
                // ---------------------------------------------------------------------
                // You have to pass the connection (con) to the command so it knows 
                // exactly WHICH telephone to speak into.
                //
                // Imagine your massive Call Center app connects to 5 different databases:
                // - A Cat Database
                // - A Dog Database
                // - A Medical Database
                // 
                // If you just yell "sp_GetAllCats" into the air, nothing happens. 
                // By passing "con", you are saying: "Read this specific order ticket 
                // INTO the Cat Database telephone line!"

                // ---------------------------------------------------------------------
                // 3. WHAT IF THE STORED PROCEDURE NAME IS WRONG? (THE KITCHEN YELLS)
                // ---------------------------------------------------------------------
                // Let's say you make a typo and write "sp_GetAllTheCats". 
                // 
                // What happens? 
                // 1. C# reads the ticket over the phone to SQL Server.
                // 2. SQL Server looks at its recipe book, can't find it, and throws a fit.
                // 3. It yells an Error back over the phone: "Could not find stored procedure!"
                // 4. Your code immediately crashes down into the "catch (Exception ex)" block.
                // 5. The Janitor (the "using" keyword) hangs up the phone.
                // 6. ControllerBase packages the Kitchen's error into a 500 StatusCode Box 
                //    and ships it to the user so they know what broke!
                // =====================================================================
                using SqlCommand cmd = new SqlCommand("sp_ViewAllCats", con);

                // =====================================================================
                // 🌟 THE MASTER GUIDE TO: cmd.CommandType 🌟
                // =====================================================================

                // ---------------------------------------------------------------------
                // 1. WHAT DOES THIS LINE DO? (THE INSTRUCTION)
                // ---------------------------------------------------------------------
                // SqlCommand needs to know HOW to read your order ("sp_GetAllCats"). 
                // By setting this, you tell SQL Server: "Don't run this as raw text. 
                // Look for a saved recipe (Stored Procedure) with this name."

                // ---------------------------------------------------------------------
                // 2. WHY IS "CommandType" WRITTEN TWICE? (THE JOB FORM ANALOGY)
                // ---------------------------------------------------------------------
                // Code: cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //
                // It looks weird because Microsoft named the Property and the Enum 
                // the exact same thing! Think of filling out a Job Application form:
                //
                // 📄 cmd.CommandType (The Blank Box)
                //    -> The empty box on your form labeled "Command Type: _______"
                //
                // 📋 System.Data.CommandType (The Cheat Sheet)
                //    -> Microsoft's official multiple-choice list. It has 3 options:
                //       1. Text (Raw SQL)
                //       2. StoredProcedure 
                //       3. TableDirect
                //
                // ✏️ .StoredProcedure (Your Answer)
                //    -> The specific choice you picked from the list.
                //
                // Translation: "Fill the blank box (cmd.CommandType) with the answer 
                // 'StoredProcedure' from the official cheat sheet (System.Data.CommandType)."

                // ---------------------------------------------------------------------
                // 3. WHY NO "new" KEYWORD?
                // ---------------------------------------------------------------------
                // Because CommandType is an ENUM, not a Class. 
                // Enums are just simple multiple-choice lists (like Role.Admin). You 
                // don't "build" an enum with 'new', you just point to an existing choice.

                // ---------------------------------------------------------------------
                // 4. WHAT IS "System.Data"? (THE FOLDER)
                // ---------------------------------------------------------------------
                // It is the Namespace (folder) where Microsoft saved this Enum. 
                // If you add "using System.Data;" at the top of your file, you can 
                // drop the folder name and shorten the line to just: 
                //      cmd.CommandType = CommandType.StoredProcedure;
                // =====================================================================
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                // SqlDataReader is a CLASS made by Microsoft
                // Its job: hold the RESULTS that came back from SQL Server
                //          and let you read them row by row
                //
                // cmd.ExecuteReader()
                // → this method is ON the SqlCommand class
                // → it does TWO things at once:
                //   THING 1: EXECUTES the stored procedure on SQL Server
                //   THING 2: RETURNS the results as a SqlDataReader object
                //
                // WHY NO "new" KEYWORD FOR SqlDataReader?
                // → because WE are not creating the SqlDataReader
                // → ExecuteReader() creates it INTERNALLY and RETURNS it to us
                // → we just RECEIVE it and store it in the variable "reader"
                // → same logic as:
                //   string upper = name.ToUpper()
                //   → ToUpper() is a method ON string
                //   → it RETURNS a string
                //   → you store the returned string in a string variable
                //   → you don't write: string upper = new string(name.ToUpper())
                //
                // → ExecuteReader() is a method ON SqlCommand
                // → it RETURNS a SqlDataReader
                // → you store the returned SqlDataReader in a SqlDataReader variable
                // → you don't write: SqlDataReader reader = new SqlDataReader(cmd.ExecuteReader())
                //
                // WHY STORE IN SqlDataReader AND NOT SqlCommand?
                // → because ExecuteReader() RETURNS type SqlDataReader — not SqlCommand
                // → whatever type a method returns → that is the type you store it in
                // → SqlCommand's job was to SEND the instruction — that job is done
                // → SqlDataReader's job is to HOLD and READ the results — new job, new class
                //
                // WHAT DOES "reader" CONTAIN AFTER THIS LINE?
                // → it contains the result table from sp_ViewAllCats:
                //   ┌────────┬──────────┬─────────┬─────┬───────┐
                //   │ CatID  │ Name     │ Breed   │ Age │ Color │
                //   ├────────┼──────────┼─────────┼─────┼───────┤
                //   │ 1      │ Whiskers │ Persian │ 3   │ White │
                //   │ 2      │ Shadow   │ Bombay  │ 2   │ Black │
                //   │ 3      │ Mochi    │ Ragdoll │ 1   │ Grey  │
                //   └────────┴──────────┴─────────┴─────┴───────┘
                // → but reader does NOT give all rows at once
                // → it has a cursor that starts BEFORE row 1
                // → you use reader.Read() to move the cursor and read one row at a time
                //
                // HOW reader.Read() WORKS IN THE while LOOP:
                // → reader.Read() does 2 things:
                //   1. Moves cursor to the NEXT row
                //   2. Returns TRUE if a row exists, FALSE if no more rows
                //
                // → while(reader.Read()) means:
                //   "keep looping as long as there are more rows"
                //
                // → example with 3 cats in the table:
                //   LOOP 1: reader.Read() → moves to row 1 → returns TRUE  → enter loop → read row 1
                //   LOOP 2: reader.Read() → moves to row 2 → returns TRUE  → enter loop → read row 2
                //   LOOP 3: reader.Read() → moves to row 3 → returns TRUE  → enter loop → read row 3
                //   LOOP 4: reader.Read() → no more rows   → returns FALSE → loop ENDS
                using SqlDataReader reader = cmd.ExecuteReader();

                // =====================================================================
                // HOW DOES while(reader.Read()) WORK?
                // =====================================================================
                // reader holds ALL the results from the stored procedure.
                // Think of reader like a cursor pointing at rows.
                // Initially it points BEFORE the first row.
                //
                // reader.Read() does 2 things:
                // 1. Moves the cursor to the NEXT row
                // 2. Returns TRUE if there was a row, FALSE if no more rows
                //
                // So while(reader.Read()) means:
                // "Keep going as long as there are more rows to read"
                // When all rows are read, reader.Read() returns false → loop ends
                //
                // Think of it like this in SQL terms:
                // Row 1: reader.Read() → moves to row 1 → returns true → enter loop
                // Row 2: reader.Read() → moves to row 2 → returns true → enter loop
                // Row 3: reader.Read() → moves to row 3 → returns true → enter loop
                // No more rows: reader.Read() → returns false → loop ends
                while (reader.Read())
                {
                    // =====================================================================
                    // WHY DO WE ADD TO THE LIST?
                    // =====================================================================
                    // You asked: "I already have cats in the table, 
                    // why do I need to add them to a list?"
                    //
                    // The database has the cats. But the DATABASE cannot be sent to the browser.
                    // We need to bring the data from the database INTO C# memory first.
                    // Then we can send it to the browser as JSON.
                    //
                    // The flow is:
                    // SQL Database → C# List<Cat> → JSON → Browser
                    //
                    // So we're not adding NEW cats. We're COPYING the data
                    // from the database into a C# list so we can send it to the browser.

                    cats.Add(new Cat
                    {
                        // =====================================================================
                        // WHY Convert.ToInt32 FOR CatID?
                        // =====================================================================
                        // reader["CatID"] → gets value from column "CatID" in CURRENT row
                        // returns type "object" — not int, not string — just generic "object"
                        // because SqlDataReader doesn't know your column types at compile time
                        // Convert.ToInt32() → converts that object to a proper C# int
                        // because CatID in Cat.cs is defined as int — types must match
                        // in the column named "CatID"
                        CatID = Convert.ToInt32(reader["CatID"]),

                        // reader["Name"] returns object → .ToString() converts to string
                        // Name in Cat.cs is string → so we use .ToString()
                        // WHY ToString() HERE BUT Convert.ToInt32() ABOVE?
                        // → CatID is int in Cat.cs → need Convert.ToInt32()
                        // → Name is string in Cat.cs → need .ToString()
                        // → always match the type in Cat.cs
                        Name = reader["Name"].ToString(),

                        Breed = reader["Breed"].ToString(),

                        Age = Convert.ToInt32(reader["Age"]),

                        Color = reader["Color"].ToString(),

                        // reader["Status"] returns "Available" or "Adopted" as text because thats how it is stored in the database with the coulmn name status
                        // .ToString() converts the object to string → "Available" or "Adopted"
                        // == "Adopted" compares it → returns TRUE or FALSE
                        // IsAdopted in Cat.cs is bool → so it gets TRUE or FALSE
                        //
                        // Example:
                        // Status = "Adopted"   → "Adopted" == "Adopted"   → TRUE  → IsAdopted = true
                        // Status = "Available" → "Available" == "Adopted" → FALSE → IsAdopted = false
                        //
                        // WHY NOT JUST reader["IsAdopted"]?
                        // Because sp_ViewAllCats returns a column called "Status" with text values
                        // not a column called "IsAdopted" with 0/1 values
                        // so we convert the text to bool ourselves with this comparison
                        IsAdopted = reader["Status"].ToString() == "Adopted"

                        // WHY ONLY 6 VALUES WHEN Cat HAS 7 PROPERTIES?
                        // EntryDate is the 7th property.
                        // Our sp_ViewAllCats doesn't return EntryDate column.
                        // So we just don't set it — it defaults to DateTime.MinValue.
                        // Not a problem for now.
                    });
                }

                // =====================================================================
                // WHY IS return Ok(cats) AFTER THE LOOP?
                // =====================================================================
                // You asked: "for every cat it finds it should return"
                // NO! If we returned inside the loop, we'd return after the FIRST cat
                // and never read the rest.
                //
                // The loop fills the list with ALL cats first.
                // THEN after the loop, we return the COMPLETE list once.
                //
                // WHAT DOES Ok(cats) DO EXACTLY?
                // Ok() is a method from ControllerBase (inherited).
                // Ok(cats) does 3 things:
                // 1. Sets status code to 200 (success)
                // 2. Takes the cats list as parameter
                // 3. Automatically converts List<Cat> to JSON format
                // 4. Sends the JSON back to the browser
                //
                // YOU ASKED: "why do we need Ok()? Can't I just write my own message?"
                // You CAN write your own message with StatusCode():
                // return StatusCode(200, cats);  ← does the same thing
                // But Ok() is a shortcut Microsoft provided for the most common case.
                //
                // DOES Ok() WORK FOR EVERY DATA TYPE?
                // YES! You can pass any object to Ok() and ASP.NET converts it to JSON.
                // Ok(cats)     → converts List<Cat> to JSON array
                // Ok(singleCat) → converts one Cat object to JSON object
                // Ok("hello")  → sends the text "hello"
                // Ok(42)       → sends the number 42
                //
                // WHY NO foreach LOOP TO PRINT?
                // Console.WriteLine = prints to the black terminal window
                // Nobody on the internet sees that.
                // Ok(cats) = Does not prints the data it SENDS data over HTTP to whoever made the request
                // the browser/Swagger receives it
                // ASP.NET handles the conversion and sending automatically.

                //Ok() is a method from ControllerBase.It does 3 things:
                //STEP 1: Takes your List < Cat >
                //STEP 2: Converts it to JSON automatically
                //(ASP.NET does this — you write zero code for it)
                //STEP 3: Packages it with status code 200
                //STEP 4: Sends the whole package back to whoever made the request
                //The JSON it sends looks like this:
                //json[
                //  { "catID": 1, "name": "Whiskers", "breed": "Persian" },
                //  { "catID": 2, "name": "Shadow",   "breed": "Bombay"  }
                //]
                //Swagger receives this JSON and DISPLAYS it on screen. That's why you saw it in Swagger —
                //Swagger received the JSON and rendered it for you.

                //Why no foreach loop?
                //Ok() internally handles the entire list.ASP.NET loops through it,
                //converts every item to JSON, and sends it all. You don't write the
                //loop because ASP.NET already wrote it for you inside Ok().
                return Ok(cats);
            }
            catch (Exception ex)
            {
                // =====================================================================
                // WHAT IS StatusCode(500, ...)?
                // =====================================================================
                // StatusCode() is another method from ControllerBase.
                // It takes 2 parameters:
                // 1. The HTTP status code number (500 = Internal Server Error)
                // 2. The message/data to send back
                //
                // WHAT DO STATUS CODE NUMBERS MEAN?
                // 200 = OK (everything worked)
                // 201 = Created (new thing was created successfully)
                // 400 = Bad Request (client sent wrong data)
                // 404 = Not Found (the thing they asked for doesn't exist)
                // 500 = Internal Server Error (something broke on YOUR server)
                //
                // WHY ex.Message?
                // ex is the Exception object that was caught.
                // ex.Message is the error description text inside it.
                // Example exceptions that can come here:
                // - SqlException: "Cannot open connection to server" (server is down)
                // - SqlException: "Invalid object name 'sp_ViewAllCats'" (wrong proc name)
                // - InvalidCastException: "Cannot convert DBNull to Int32" (null in database)
                //
                // We return 500 + the error message so the browser knows:
                // "Something broke on the server AND here's why"
                return StatusCode(500, "Database error: " + ex.Message);
            }
        }

        // =====================================================================
        // AddCat METHOD
        // =====================================================================
        // =====================================================================
        // 🌟 THE MASTER GUIDE TO: [HttpPost] 🌟
        // =====================================================================

        // ---------------------------------------------------------------------
        // WHAT IS [HttpPost]? (THE ADMISSIONS DESK)
        // ---------------------------------------------------------------------
        // → [HttpGet] is the "Information Desk" (Give me data / READ)
        // → [HttpPost] is the "Admissions Desk" (Here is new data / CREATE)
        //
        // → When a user fills out a form on the website and clicks "Save", 
        //   the browser sends a POST request.
        // → ASP.NET sees the word "POST", completely ignores the [HttpGet] door, 
        //   and routes the traffic directly to THIS method.

        // ---------------------------------------------------------------------
        // WHY IS THERE NO EXTRA URL NEEDED? (THE VERB LOGIC)
        // ---------------------------------------------------------------------
        // You noticed it doesn't say [HttpPost("add-cat")]. It just says [HttpPost].
        // WHY?
        //
        // → The URL (/api/cats) is the ADDRESS of the building.
        // → The HTTP VERB (GET vs POST) is the INTENTION of why you are there.
        //
        // → The Browser sends a GET to /api/cats
        //   "I am at the cat building. Please GIVE ME the list of cats."
        //
        // → The Browser sends a POST to /api/cats
        //   "I am at the cat building. Please TAKE THIS BOX and save it inside."
        //
        // → Because the browser's "intention" (the verb) is different, ASP.NET 
        //   never gets confused. You can use the exact same URL for both!

        // ---------------------------------------------------------------------
        // HOW DOES THE DATA GET HERE?
        // ---------------------------------------------------------------------
        // → When the browser sends a GET request, it usually travels empty-handed.
        // → When the browser sends a POST request, it acts like a delivery truck.
        // → It hides the new Cat data (JSON) inside the "Body" of the HTTP request 
        //   (the hidden cargo space).
        // → Next, we will use [FromBody] to open that cargo space and grab the cat!
        // =====================================================================
        [HttpPost]

        // =====================================================================
        // 🌟 THE ULTIMATE MASTER GUIDE: HTTP POST, ROUTING, & [FromBody] 🌟
        // =====================================================================

        // ---------------------------------------------------------------------
        // 1. THE BIG PICTURE: CLIENT VS. SERVER
        // ---------------------------------------------------------------------
        // The biggest trap is confusing the Internet (Swagger/Browser) with 
        // your Code (C#). 
        //
        // → THE CLIENT (Swagger): Packs the data, creates the HTTP Request, 
        //   and drives it across the internet cables (Wi-Fi).
        // → THE SERVER (Your C# Code): Contains ZERO data initially. It is just 
        //   an empty building waiting for the delivery truck to arrive.
        //
        // 🛑 MASSIVE REALIZATION:
        // [FromBody] DOES NOT pack the envelope. It has ZERO power over the internet.
        // By the time [FromBody] even realizes what is happening, the data has 
        // already been packed and delivered to the ASP.NET Mailroom floor.

        // ---------------------------------------------------------------------
        // 2. WHAT IS AN HTTP REQUEST REALLY? (The Envelope & The Letter)
        // ---------------------------------------------------------------------
        // Think of an HTTP Request as a physical piece of mail. It has TWO parts:
        //
        // 1. THE HEADERS (The Outside of the Envelope):
        //    → This has the address (the URL: /api/cats).
        //    → It tells the internet post office exactly where to go.
        // 
        // 2. THE BODY (The Letter Inside):
        //    → This is the folded-up piece of paper hidden INSIDE the envelope.
        //    → Because writing a massive JSON object on the outside of an envelope 
        //      (the URL) is messy and has strict size limits, the browser securely 
        //      hides heavy data inside the "Body".

        // ---------------------------------------------------------------------
        // 3. WHY DID "GET" NOT USE AN ENVELOPE? (Postcard vs. Package)
        // ---------------------------------------------------------------------
        // → HTTP GET = A POSTCARD.
        //   If you mail a postcard, there is no "inside" to open. You just write 
        //   "Send me the cats!" on the back next to the address. You don't 
        //   need a heavy envelope just to ask a question.
        //
        // → HTTP POST = A FEDEX PACKAGE.
        //   You are creating something new. You are shipping a physical object 
        //   (the JSON data for the new cat). You MUST put it INSIDE a package. 

        // ---------------------------------------------------------------------
        // 4. THE 3 WAYS TO SEND DATA (The 3 Pockets)
        // ---------------------------------------------------------------------
        // You absolutely CAN put data in the URL! There are 3 distinct ways:
        //
        // → METHOD 1: IN THE URL PATH (The Door Number) -> [FromRoute]
        //   URL: /api/cats/5  (ASP.NET grabs the "5" directly from the address).
        //
        // → METHOD 2: IN THE URL QUERY (The Post-It Note) -> [FromQuery]
        //   URL: /api/cats/search?color=Black 
        //   (Think of this as a sticky note slapped on the outside of the envelope).
        //
        // → METHOD 3: INSIDE THE ENVELOPE (The Letter) -> [FromBody]
        //   URL: /api/cats  |  Body: { "name": "Tiger" }
        //   (The URL is clean, and the heavy data is hidden inside the envelope).

        // ---------------------------------------------------------------------
        // 5. THE MULTIPLE DOORS PROBLEM (Ambiguous Match & Routing)
        // ---------------------------------------------------------------------
        // "What if there are multiple functions with POST requests?"
        //
        // → If you have two doors that just say [HttpPost], the Delivery Driver 
        //   will panic. ASP.NET throws an "AmbiguousMatchException" and crashes 
        //   because it refuses to guess which door to open!
        //
        // → THE SOLUTION: We add specific names to the signs on the door (Routing).
        //   DOOR 1: [HttpPost("add")]
        //   DOOR 2: [HttpPost("feed")]
        //
        // → Now, the driver goes specifically to `/api/cats/add`, walks down the 
        //   hallway, finds the door specifically labeled "add", and walks inside.

        // ---------------------------------------------------------------------
        // 6. [HttpPost] vs [FromBody] (The Door vs The Box)
        // ---------------------------------------------------------------------
        // "Since the HTTP request contains the data, shouldn't they be written together?"
        // NO, because they do two completely different jobs at two different times!
        //
        // → Step 1: [HttpPost] is the TRAFFIC COP. 
        //   It sits above the method. It only cares about routing. It gets the 
        //   driver into the correct room but DOES NOT look at the data.
        //
        // → Step 2: [FromBody] is the DATA EXTRACTOR. 
        //   It sits next to the variable. Once the driver is in the room, it tells 
        //   the blind Mailroom Worker exactly how to unpack the truck.

        // ---------------------------------------------------------------------
        // 7. WHAT EXACTLY DOES [FromBody] DO? (The Sticky Note)
        // ---------------------------------------------------------------------
        // It only has TWO jobs. It is just a bright yellow STICKY NOTE for ASP.NET.
        //
        // JOB 1: "Where do I look?"
        // → Worker: "A truck arrived! Where is the data? URL? Query?"
        // → [FromBody]: "Stop panicking. Ignore the URL. Cut open the main envelope 
        //   and look strictly inside the BODY."
        //
        // JOB 2: "Where do I put it?"
        // → Worker: "Okay, I translated the JSON into a C# Cat object. Now what?"
        // → [FromBody]: "See the variable I am physically touching? Put it exactly 
        //   into this specific box."

        // ---------------------------------------------------------------------
        // 8. VARIABLE PLACEMENT & THE 2-PARAMETER RULE
        // ---------------------------------------------------------------------
        // "Will I make two [FromBody] tags if I have two variables?"
        //
        // 🛑 NO! You can ONLY have ONE [FromBody] per method!
        //
        // → WHY? (The Single Cargo Bay Rule)
        //   An HTTP Request only has ONE main cargo bay. Once ASP.NET opens 
        //   the Body and reads the JSON text, the stream is "consumed" (deleted 
        //   from memory to save space). You physically cannot read the Body twice!
        //
        // → THE FIX (The Master Crate)
        //   If you need to send a Cat and a Dog, you CANNOT do this:
        //   AddAnimals([FromBody] Cat cat, [FromBody] Dog dog) // ❌ CRASHES!
        //
        //   You must pack them into one bigger box before shipping:
        //   AddAnimals([FromBody] PetDelivery delivery) // ✅ WORKS!

        // ---------------------------------------------------------------------
        // 9. MIXING & STACKING (Advanced Worker Instructions)
        // ---------------------------------------------------------------------
        // "Can I mix [FromRoute] and [FromBody] in the same method?"
        // 
        // → YES! Example: UpdateCat([FromRoute] int id, [FromBody] Cat newCat)
        // 
        // → DOES THIS UPDATE TWO CATS? 
        //   NO. It is the "Address and The Paint" analogy:
        //   1. [FromRoute] int id  = THE ADDRESS (SQL, find Cat #5).
        //   2. [FromBody] Cat cat  = THE PAINT (SQL, apply this new data to Cat #5).
        // 
        // → WHAT IF THE ID IN THE URL AND THE BODY DON'T MATCH?
        //   If URL says ID 5, but the Body JSON says ID 99, you have a security risk!
        //   You must write an `if (id != newCat.CatId)` check to reject the request.
        //
        // "Can I stack multiple attributes on one parameter?"
        // → YES! Example: AddCat([FromBody][Required] Cat cat)
        // → This slaps two sticky notes on the box: "Get it from the Body" AND 
        //   "If the Body is empty, sound the alarm and reject the delivery!"

        // ---------------------------------------------------------------------
        // 10. THE FULL JOURNEY OF A CAT (Start to Finish)
        // ---------------------------------------------------------------------
        // Here is the exact, step-by-step path from the internet to the database:
        //
        // 🌐 STEP 1: THE USER (Swagger)
        // → Types the JSON: { "name": "Tiger", "breed": "Bengal" }
        //
        // 🚚 STEP 2: THE DRIVER (The HTTP Request)
        // → Swagger takes your JSON, folds it up (The Body), puts it inside an 
        //   HTTP POST envelope, and drives it across the internet to: /api/cats
        //
        // 🏢 STEP 3: THE MAILROOM (ASP.NET)
        // → ASP.NET receives the envelope. 
        // → It looks at the [HttpPost] sign and walks through the correct door.
        // → It reads the [FromBody] sticky note next to the variable.
        // → It follows the instruction: opens the envelope, translates the JSON, 
        //   and automatically builds a C# "Cat" object, BECAUSE [FromBody] is sitting
        //  next to the Cat cat, Cat is the class cat is the name of the object of that
        //  class in which the dats is to be stored
        //
        // 💻 STEP 4: YOUR C# CODE
        // → ASP.NET hands your AddCat() method the finished C# Cat object. 
        // → (You didn't have to parse a single line of JSON text yourself!)
        //
        // 🗄️ STEP 5: THE DATABASE (SQL)
        // → You grab a telephone line from the Pool (SqlConnection).
        // → You write an order ticket (SqlCommand) with an "INSERT" query.
        // → You pass the data from your C# Cat object into the order ticket.
        // → You press Send (ExecuteNonQuery).
        // → SQL Server adds a brand new row to the Cat Table!
        // =====================================================================
        public IActionResult AddCat([FromBody] Cat cat)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                con.Open();

                // =====================================================================
                // 🌟 THE MASTER GUIDE TO: STORED PROCEDURES & PARAMETERS 🌟
                // =====================================================================

                // ---------------------------------------------------------------------
                // 1. THE COMMAND (Writing the Order Ticket)
                // ---------------------------------------------------------------------
                // Code: using SqlCommand cmd = new SqlCommand("sp_AddCat", con);
                //
                // → Just like before, we are writing an Order Ticket for the SQL Kitchen.
                // → But notice we just wrote "sp_AddCat". If SQL looks at that, it might 
                //   get confused and think it's just a random word instead of a command.

                // ---------------------------------------------------------------------
                // 2. WHAT IS CommandType.StoredProcedure? (The Menu Item)
                // ---------------------------------------------------------------------
                // Code: cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //
                // → If you send raw SQL text (like "INSERT INTO Cats..."), you are 
                //   giving the chef step-by-step instructions.
                //
                // → A "Stored Procedure" is a Pre-Saved Recipe that the database has 
                //   already memorized. 
                //
                // → By adding this line, you are telling SQL Server: 
                //   "Do not try to read 'sp_AddCat' as a raw command. It is the name of 
                //   a recipe you already have saved in your cookbook. Go find it!"

                // ---------------------------------------------------------------------
                // 3. WHAT IS Parameters.AddWithValue? (Filling in the Blanks)
                // ---------------------------------------------------------------------
                // Code: cmd.Parameters.AddWithValue("@Name", cat.Name);
                //
                // → The kitchen knows the recipe for "sp_AddCat", but it is a custom 
                //   recipe. It has blank spaces that require specific ingredients.
                //
                // → In SQL, those blank spaces are called parameters, and they always 
                //   start with an @ symbol (like @Name, @Breed, @Age).
                //
                // → AddWithValue() literally translates to:
                //   "On the kitchen's order ticket, find the blank space labeled '@Name', 
                //   and write the value of 'cat.Name' inside of it."

                // ---------------------------------------------------------------------
                // 4. CONNECTING THE DOTS: WHERE DID "cat.Name" COME FROM?
                // ---------------------------------------------------------------------
                // You asked: "Where did 'cat' come from?"
                //
                // Let's connect the Mailroom to the Kitchen!
                //
                // 🚚 1. The Delivery Truck (Swagger) hid { "name": "Tiger" } in the Body.
                // 🏢 2. The Mailroom Worker ([FromBody]) opened the envelope, converted 
                //       it, and put it perfectly into the C# variable box named `cat`.
                // 👨‍🍳 3. NOW, YOU (the developer) are reaching into that `cat` box, 
                //       pulling out the Name property ("Tiger"), and stapling it to the 
                //       SQL Kitchen's order ticket (`@Name`).
                //
                // → When you call cmd.ExecuteNonQuery(), the Waiter walks into the kitchen 
                //   holding an order ticket that says: "Cook recipe sp_AddCat, and use 
                //   the ingredient 'Tiger' for the @Name."
                // =====================================================================
                using SqlCommand cmd = new SqlCommand("sp_AddCat", con);
                cmd.Parameters.AddWithValue("@Name", cat.Name);
                cmd.Parameters.AddWithValue("@Breed", cat.Breed);
                cmd.Parameters.AddWithValue("@Age", cat.Age);
                cmd.Parameters.AddWithValue("@Color", cat.Color);

                // =====================================================================
                // 🌟 THE MASTER GUIDE TO: ExecuteNonQuery vs ExecuteReader 🌟
                // =====================================================================

                // ---------------------------------------------------------------------
                // 1. THE BIG DIFFERENCE (Asking for Data vs. Giving a Command)
                // ---------------------------------------------------------------------
                // → ExecuteReader = ASKING FOR DATA (SELECT)
                //   You use this when your SQL recipe (sp_ViewAllCats) is supposed to 
                //   give you rows of data back. The database builds a Conveyor Belt 
                //   (SqlDataReader) to stream the cats back to your C# code.
                //
                // → ExecuteNonQuery = COMMANDING AN ACTION (INSERT, UPDATE, DELETE)
                //   You use this when your SQL recipe (sp_AddCat) just needs to DO 
                //   work inside the database, but doesn't have any actual data to hand back.

                // ---------------------------------------------------------------------
                // 2. WHY NOT ExecuteReader FOR sp_AddCat? (The Empty Conveyor Belt)
                // ---------------------------------------------------------------------
                // → sp_AddCat runs an "INSERT INTO Cats" command. 
                // → It is taking the C# cat data you passed in and securely saving it 
                //   into the database tables.
                // → Because it is only ADDING data, SQL Server has zero rows to send back!
                // → If you tried to use ExecuteReader here, you would be forcing the 
                //   database to build a massive Conveyor Belt for absolutely no reason, 
                //   because nothing is coming back. It would just be an empty belt!

                // ---------------------------------------------------------------------
                // 3. WHAT DOES "NonQuery" ACTUALLY MEAN? (The Kitchen Analogy)
                // ---------------------------------------------------------------------
                // → "Query" in this specific context means: "A request that returns a table."
                // → "NonQuery" literally translates to: "Execute this command, but do NOT 
                //   expect a table of results back."
                // 
                // → Think of it like giving a command to the Kitchen:
                //   - ExecuteReader: "Make me a pizza!" 
                //     (The Kitchen cooks and hands back a physical pizza).
                //
                //   - ExecuteNonQuery: "Clean the oven!" 
                //     (The Kitchen doesn't hand an oven back to you. They just do the 
                //     work and give you a Thumbs Up when they are finished).

                // ---------------------------------------------------------------------
                // 4. WHAT DOES ExecuteNonQuery RETURN? (The Receipt)
                // ---------------------------------------------------------------------
                // → Even though it doesn't return a table of data, ExecuteNonQuery DOES 
                //   return one tiny thing: an integer (a whole number).
                // → This number is the "Rows Affected".
                //
                // → Example: int rowsAffected = cmd.ExecuteNonQuery();
                //
                // → If you inserted 1 cat, SQL says: "Thumbs up! I changed 1 row."
                // → If you updated 50 cats, SQL says: "Thumbs up! I changed 50 rows."
                // → If it failed, it returns 0. 
                //
                // → You don't always have to save this number in a variable, but it is 
                //   the database's way of giving you a receipt that the job was actually 
                //   completed successfully!
                // =====================================================================
                cmd.ExecuteNonQuery();

                // =====================================================================
                // 🌟 THE MASTER GUIDE TO: Ok("Message") vs SQL PRINT 🌟
                // =====================================================================

                // ---------------------------------------------------------------------
                // 1. WHERE DOES SQL "PRINT" GO? (The Soundproof Kitchen)
                // ---------------------------------------------------------------------
                // → If your stored procedure has `PRINT 'Cat added!';`, it literally 
                //   just yells that sentence into the SQL Server console.
                //
                // → Think of SQL Server as a soundproof kitchen. The chef (SQL) yells 
                //   "Done!", but the Delivery Driver (Swagger) is waiting outside the 
                //   building and cannot hear a single word.
                //
                // → NOBODY on the internet will ever see a SQL PRINT message. It is 
                //   strictly for the database administrator looking at the SQL screen.

                // ---------------------------------------------------------------------
                // 2. THE DIFFERENCE BETWEEN Ok() AND Ok("Message")
                // ---------------------------------------------------------------------
                // → Code: return Ok();
                //   The ASP.NET Mailroom Worker just gives the Delivery Driver a 
                //   "Thumbs Up" (HTTP Status 200). The driver goes back to Swagger 
                //   and says, "It worked!" but has no extra details.
                //
                // → Code: return Ok("Cat added successfully!");
                //   The Worker gives a "Thumbs Up" (HTTP Status 200), BUT they also 
                //   write a receipt, put it in a brand new return envelope, and hand 
                //   it to the driver to take back to the Boss.

                // ---------------------------------------------------------------------
                // 3. WHY WE SEND OUR OWN MESSAGE (The Return Envelope)
                // ---------------------------------------------------------------------
                // → The HTTP Request was the envelope coming IN.
                // → The HTTP Response (`return Ok(...)`) is the envelope going OUT.
                //
                // → By putting text inside Ok(), you are taking control of the communication. 
                //   You are creating a Response Body. 
                // → When the driver gets back to Swagger, Swagger opens the return 
                //   envelope and proudly displays your custom text on the screen!
                // =====================================================================
                return Ok("Cat added successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Database error: " + ex.Message);
            }
        }

        // =====================================================================
        // AdoptCat METHOD
        // =====================================================================
        // =====================================================================
        // 🌟 THE MASTER GUIDE TO: [HttpPut("{id}")], AUTO-BINDING, & ROUTING 🌟
        // =====================================================================

        // ---------------------------------------------------------------------
        // 1. WHY DOES PUT NEED "{id}" WHEN POST DIDN'T? (New vs. Existing)
        // ---------------------------------------------------------------------
        // → POST (Create): You are dropping off a brand NEW cat (The Birth). 
        //   It doesn't have an ID yet because the database hasn't created the row. 
        //   You just hand over the data (in the Body), and the server figures it out.
        //   URL: /api/cats
        //
        // → PUT (Update): You are modifying an EXISTING cat. 
        //   If you just send a request saying "Change the status to Adopted!", 
        //   the server panics. "WHICH cat am I adopting?!" 
        //   You MUST provide the exact ID in the URL to target a specific row.
        //   URL: /api/cats/2

        // ---------------------------------------------------------------------
        // 2. WHAT IS "{id}"? (The Route Catching Mitt)
        // ---------------------------------------------------------------------
        // Code: [HttpPut("{id}")]
        //
        // → The curly braces {} act as a "Catching Mitt" for ASP.NET routing. 
        // → It literally means: "The user is going to type a value at the end 
        //   of the URL. Whatever they type, catch it, and label it 'id'."
        //
        // → Example URL: /api/cats/2
        //   ASP.NET catches the "/2", extracts it, and says: "Okay, id = 2".

        // ---------------------------------------------------------------------
        // 3. WHY DID I NOT NEED [FromRoute]? (The Auto-Magic Match)
        // ---------------------------------------------------------------------
        // Code: public IActionResult AdoptCat(int id)
        //
        // You might ask: "Why didn't I have to put a sticky note here?"
        //
        // → Because of ASP.NET's "Auto-Magic" Rule: 
        //   If the name in the curly braces "{id}" matches the EXACT name of the 
        //   C# variable "int id", ASP.NET assumes you want them connected!
        //
        // → It secretly puts an invisible `[FromRoute]` tag there for you.
        //   Writing `AdoptCat(int id)` is exactly the same as writing 
        //   `AdoptCat([FromRoute] int id)`. ASP.NET just lets you be lazy and 
        //   skip writing it if the names match perfectly.

        // ---------------------------------------------------------------------
        // 4. WHAT IF I CHANGE THE VARIABLE NAME? (The Disconnect)
        // ---------------------------------------------------------------------
        // "What if I changed the variable name?" Let's see how ASP.NET breaks!
        //
        // Code:
        // [HttpPut("{id}")]
        // public IActionResult AdoptCat(int catNumber)
        //
        // → DRIVER: Arrives with URL `/api/cats/2`.
        // → WORKER: Looks at the door placeholder `{id}`. It holds the number 2.
        // → WORKER: Looks at your C# variable `catNumber`.
        // → WORKER: "Wait... the URL says 'id', but this box says 'catNumber'. 
        //   These names don't match! I am not going to guess!"
        //
        // 💥 RESULT: ASP.NET drops the data on the floor. Your `catNumber` 
        //   variable will be exactly 0, and your database update will fail!

        // ---------------------------------------------------------------------
        // 5. HOW TO FIX DIFFERENT NAMES (Taking Control Back)
        // ---------------------------------------------------------------------
        // If you absolutely MUST have different names, you can no longer rely 
        // on the "Auto-Magic". You have to write a very specific sticky note 
        // to explicitly connect the two different names:
        //
        // Code:
        // [HttpPut("{id}")]
        // public IActionResult AdoptCat([FromRoute(Name = "id")] int catNumber)
        //
        // → This strict sticky note says: 
        //   "Hey Worker, go to the Route. Find the piece of data named 'id'. 
        //   Extract it, and shove it into this variable named 'catNumber'."
        //
        // 💡 BEST PRACTICE: Keep the names exactly the same so you don't have 
        // to write extra code!

        // ---------------------------------------------------------------------
        // 6. ROUTE vs QUERY (The Visual Difference)
        // ---------------------------------------------------------------------
        // → ROUTE (The Door Number)
        //   URL: /api/cats/2
        //   Code: [HttpPut("{id}")] 
        //   Data is physically built into the path itself.
        //
        // → QUERY (The Sticky Note on the Envelope)
        //   URL: /api/cats?id=2
        //   Code: [HttpPut] (Notice no curly braces!)
        //   Data is tagged on the end using a Question Mark (?).

        // ---------------------------------------------------------------------
        // 7. CAN I USE {id} IN [HttpPost] ALONG WITH [FromBody]? 
        // ---------------------------------------------------------------------
        // You asked: "Could I put curly braces in HttpPost and still use [FromBody]?"
        //
        // → TECHNICALLY: Yes. ASP.NET will absolutely let you write this:
        //   [HttpPost("{id}")]
        //   public IActionResult AddCat(int id, [FromBody] Cat cat)
        //
        // → LOGICALLY: You should almost NEVER do this for a POST (Add) method!
        //   If you force an ID in the URL for a POST, you are making the User 
        //   (in Swagger) guess the next available ID. If they guess an ID that 
        //   already exists, the database crashes.
        //
        // 🏆 THE GOLDEN RULE OF REST APIs:
        // → POST = ADDING. The User does NOT know the ID. Just hand over the Body 
        //   and let the Database generate the ID automatically.
        // → PUT = UPDATING. You need BOTH an Address (Route ID) and New Paint (Body).
        // =====================================================================
        [HttpPut("{id}")]
        public IActionResult AdoptCat(int id) // id comes from the URL /api/cats/2
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                con.Open();
                using SqlCommand cmd = new SqlCommand("sp_AdoptCat", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CatID", id); // pass the iStoredProcedure;d from URL to stored procedure
                cmd.ExecuteNonQuery(); // UPDATE doesn't return rows → ExecuteNonQuery
                return Ok("Cat adopted successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Database error: " + ex.Message);
            }
        }

        // DELETE method works same as PUT — needs an ID in the URL
        [HttpDelete("{id}")]
        public IActionResult DeleteCat(int id)
        {
            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                con.Open();
                using SqlCommand cmd = new SqlCommand("sp_RemoveCat", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CatID", id);
                cmd.ExecuteNonQuery();
                return Ok("Cat removed successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Database error: " + ex.Message);
            }
        }

        // =====================================================================
        // SearchCat METHOD
        // =====================================================================
        // [HttpGet("search/{name}")] — why "search/{name}"?
        // Without "search", the URL would be /api/cats/{name}
        // But /api/cats/{id} is already taken by AdoptCat!
        // ASP.NET would get confused: is "whiskers" a name or an ID?
        // So we add "search/" to make it unique: /api/cats/search/whiskers
        // Now ASP.NET clearly knows: search URL → SearchCat method
        //
        // {name} is a URL parameter. When browser visits /api/cats/search/Tiger
        // ASP.NET extracts "Tiger" and passes it as the "name" parameter below// =====================================================================
        // 🌟 THE MASTER GUIDE TO: ROUTE COLLISIONS & THE "PASSWORD" 🌟
        // =====================================================================

        // ---------------------------------------------------------------------
        // 1. THE SETUP: TWO DOORS, ONE HALLWAY
        // ---------------------------------------------------------------------
        // Imagine the Delivery Driver arrives at the API and says, "I have a GET 
        // request!" (They want to read data, not add data).
        //
        // The Worker points them down the [HttpGet] hallway. 
        // Down this hallway, you have built two different doors:
        //
        // DOOR 1 (Find by ID):   [HttpGet("{id}")]
        // DOOR 2 (Find by Name): [HttpGet("{name}")]
        //
        // Both doors have a "Catching Mitt" (curly braces). 
        // - Door 1 says: "I will catch ANY word you throw at me and call it 'id'."
        // - Door 2 says: "I will catch ANY word you throw at me and call it 'name'."

        // ---------------------------------------------------------------------
        // 2. THE DISASTER (Ambiguous Match)
        // ---------------------------------------------------------------------
        // The Delivery Driver walks down the hallway holding a piece of paper. 
        // The paper says: /api/cats/tiger
        //
        // → The Driver hands the word "tiger" to the Mailroom Worker.
        // → The Worker looks at Door 1: "Well, I can catch 'tiger' and put it 
        //   in the ID box. That fits!"
        // → The Worker looks at Door 2: "Wait, I can also catch 'tiger' and put 
        //   it in the Name box. That fits too!"
        // 
        // 💥 THE CRASH: The Worker panics. They do not know if "tiger" is supposed 
        // to be an ID number (maybe someone named their cat with numbers?) or a Name. 
        // Because ASP.NET refuses to guess, it throws an "AmbiguousMatchException" 
        // and literally crashes your application!

        // ---------------------------------------------------------------------
        // 3. THE SOLUTION: ADDING A PASSWORD (The Route Literal)
        // ---------------------------------------------------------------------
        // To stop the Worker from panicking, we have to make the doors look 
        // completely different. We do this by adding a hardcoded word (a literal) 
        // in front of the catching mitt.
        //
        // We change Door 2 to: [HttpGet("search/{name}")]
        //
        // Now, look at how the signs on the doors have changed:
        // - DOOR 1: "{id}" -> "I will catch ANY word and call it an ID."
        // - DOOR 2: "search/{name}" -> "You CANNOT use this door unless you speak 
        //   the exact password 'search' first. Then, I will catch the next word."

        // ---------------------------------------------------------------------
        // 4. HOW THE DRIVER NAVIGATES IT NOW (Crisis Averted)
        // ---------------------------------------------------------------------
        // Now, let's see what happens when the Driver arrives:
        //
        // SCENARIO A: Driver arrives with `/api/cats/2`
        // → Worker looks at the word "2". 
        // → Did the driver say the password "search"? No.
        // → Worker instantly shoves the driver through Door 1 (ID).
        //
        // SCENARIO B: Driver arrives with `/api/cats/search/tiger`
        // → Worker hears the exact password "search". 
        // → Worker instantly knows Door 2 is the only option. 
        // → Worker grabs the word after the password ("tiger"), puts it into the 
        //   `{name}` box, and successfully finds your cat!

        // ---------------------------------------------------------------------
        // 5. SUMMARY
        // ---------------------------------------------------------------------
        // → Curly Braces `{name}` act as a variable (a catching mitt). They will 
        //   catch literally anything the user types.
        // → Hardcoded words without braces `search` act as a strict path (a password).
        // → If you have multiple methods that use curly braces, you MUST add 
        //   passwords to them so ASP.NET knows exactly which door to open!
        // =====================================================================
        [HttpGet("search/{name}")] //this is the url
        public IActionResult SearchCat(string name) // name comes from URL
        {
            // WHY A NEW LIST HERE INSTEAD OF ONE LIST AT THE TOP?
            // Each method is INDEPENDENT. They run separately for different requests.
            // If 2 users call the API at the same time:
            // User 1 → GetAllCats() → their own list
            // User 2 → SearchCat() → their own list
            // If we shared one list at the top, they'd mix each other's data!
            // Each method needs its OWN clean list every time it runs.
            //
            // WHY A LIST FOR SEARCH — WHAT IF ONLY ONE CAT MATCHES?
            // What if 3 cats are named "Whiskers"? The search could return multiple.
            // A list handles 0, 1, or many results safely.
            // An object can only hold exactly 1 result.
            List<Cat> cats = new List<Cat>();

            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                con.Open();
                using SqlCommand cmd = new SqlCommand("sp_SearchCat", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                // -----------------------------------------------------------------
                // THE HAND-OFF (URL -> C# -> SQL)
                // -----------------------------------------------------------------
                // This is the magic bridge between the Internet and the Database:
                // 🌐 1. Driver arrives with URL: /api/cats/search/Tiger
                // 🏢 2. Route Catching Mitt `{name}` grabs "Tiger" and puts it 
                //       in the C# variable `string name`.
                // 👨‍🍳 3. You take "Tiger" out of the C# variable and staple it to 
                //       the SQL Order Ticket at the exact spot labeled `@Name`.
                cmd.Parameters.AddWithValue("@Name", name);

                // 4. THE CONVEYOR BELT (ExecuteReader)
                // -----------------------------------------------------------------
                // → Why ExecuteReader and not NonQuery? 
                //   Because "sp_SearchCat" is a SELECT query! The kitchen is actually 
                //   cooking food (data) and needs to send it back to you. 
                // → ExecuteReader builds the physical Conveyor Belt to stream the rows.

                // =====================================================================
                // 🌟 VISUAL DEMO: ExecuteReader & reader.Read() 🌟
                // =====================================================================

                // ---------------------------------------------------------------------
                // STEP 1: using SqlDataReader reader = cmd.ExecuteReader();
                // ---------------------------------------------------------------------
                // → ACTION: SQL Server runs the query, grabs the 3 cats, and builds 
                //   the table in memory. 
                // → THE CATCH: The "Reader Pointer" does NOT start on row 1. It starts 
                //   floating just BEFORE the first row, waiting for your command.

                // [ POINTER IS HERE ] ---> (Waiting...)
                //   ┌────────┬──────────┬─────────┬─────┬───────┐
                //   │ CatID  │ Name     │ Breed   │ Age │ Color │
                //   ├────────┼──────────┼─────────┼─────┼───────┤
                //   │ 1      │ Whiskers │ Persian │ 3   │ White │ 
                //   │ 2      │ Shadow   │ Bombay  │ 2   │ Black │ 
                //   │ 3      │ Mochi    │ Ragdoll │ 1   │ Grey  │ 
                //   └────────┴──────────┴─────────┴─────┴───────┘


                // ---------------------------------------------------------------------
                // STEP 2: The First Loop -> while (reader.Read())
                // ---------------------------------------------------------------------
                // → ACTION: The while loop calls `reader.Read()`. 
                // → The Pointer steps DOWN one row. 
                // → Did it land on real data? YES. So `reader.Read()` returns TRUE.
                // → Your C# code inside the loop now safely grabs "Whiskers" and "Persian".

                //   ┌────────┬──────────┬─────────┬─────┬───────┐
                //   │ CatID  │ Name     │ Breed   │ Age │ Color │
                //   ├────────┼──────────┼─────────┼─────┼───────┤
                // [ POINTER ]>| 1      │ Whiskers │ Persian │ 3   │ White │ (Returns TRUE)
                //   │ 2      │ Shadow   │ Bombay  │ 2   │ Black │
                //   │ 3      │ Mochi    │ Ragdoll │ 1   │ Grey  │
                //   └────────┴──────────┴─────────┴─────┴───────┘


                // ---------------------------------------------------------------------
                // STEP 3: The Second Loop -> while (reader.Read())
                // ---------------------------------------------------------------------
                // → ACTION: The loop finishes Cat #1 and loops back to the top. 
                // → It calls `reader.Read()` again. The Pointer steps DOWN.
                // → Did it land on real data? YES. So `reader.Read()` returns TRUE.
                // → Your C# code grabs "Shadow" and "Bombay".

                //   ┌────────┬──────────┬─────────┬─────┬───────┐
                //   │ CatID  │ Name     │ Breed   │ Age │ Color │
                //   ├────────┼──────────┼─────────┼─────┼───────┤
                //   │ 1      │ Whiskers │ Persian │ 3   │ White │ 
                // [ POINTER ]>| 2      │ Shadow   │ Bombay  │ 2   │ Black │ (Returns TRUE)
                //   │ 3      │ Mochi    │ Ragdoll │ 1   │ Grey  │
                //   └────────┴──────────┴─────────┴─────┴───────┘


                // ---------------------------------------------------------------------
                // STEP 4: The Third Loop -> while (reader.Read())
                // ---------------------------------------------------------------------
                // → ACTION: The loop finishes Cat #2. Calls `reader.Read()` again.
                // → The Pointer steps DOWN. It lands on Cat #3. 
                // → Returns TRUE. C# grabs "Mochi".

                //   ┌────────┬──────────┬─────────┬─────┬───────┐
                //   │ CatID  │ Name     │ Breed   │ Age │ Color │
                //   ├────────┼──────────┼─────────┼─────┼───────┤
                //   │ 1      │ Whiskers │ Persian │ 3   │ White │
                //   │ 2      │ Shadow   │ Bombay  │ 2   │ Black │
                // [ POINTER ]>| 3      │ Mochi    │ Ragdoll │ 1   │ Grey  │ (Returns TRUE)
                //   └────────┴──────────┴─────────┴─────┴───────┘


                // ---------------------------------------------------------------------
                // STEP 5: The Final Loop -> while (reader.Read())
                // ---------------------------------------------------------------------
                // → ACTION: The loop finishes Cat #3 and loops back to the top.
                // → It calls `reader.Read()` one last time. 
                // → The Pointer steps DOWN... but it falls off the edge of the table!
                // → Did it land on real data? NO. So `reader.Read()` returns FALSE.
                // → The `while(false)` loop immediately stops and skips your C# code.

                //   ┌────────┬──────────┬─────────┬─────┬───────┐
                //   │ CatID  │ Name     │ Breed   │ Age │ Color │
                //   ├────────┼──────────┼─────────┼─────┼───────┤
                //   │ 1      │ Whiskers │ Persian │ 3   │ White │
                //   │ 2      │ Shadow   │ Bombay  │ 2   │ Black │
                //   │ 3      │ Mochi    │ Ragdoll │ 1   │ Grey  │
                //   └────────┴──────────┴─────────┴─────┴───────┘
                // [ POINTER IS HERE ] ---> (Empty Void. Returns FALSE. Loop breaks!)

                // ---------------------------------------------------------------------
                // SUMMARY: WHY IT WORKS THIS WAY
                // ---------------------------------------------------------------------
                // This design is incredibly memory efficient. The computer NEVER tries to 
                // load all 3 cats into C# memory at the exact same time. 
                // It only loads the EXACT row the Pointer is pointing at, processes it, 
                // deletes it from active memory, and moves to the next one!
                // =====================================================================
                using SqlDataReader reader = cmd.ExecuteReader();

                // reader.Read() moves to next row and returns true/false
                // Loop runs once per row returned by the stored procedure
                while (reader.Read())
                {
                    cats.Add(new Cat
                    {
                        // Same as GetAllCats — read each column from current row
                        // Convert.ToInt32 for int columns, .ToString() for string columns
                        CatID = Convert.ToInt32(reader["CatID"]),
                        Name = reader["Name"].ToString(),
                        Breed = reader["Breed"].ToString(),
                        Age = Convert.ToInt32(reader["Age"]),
                        Color = reader["Color"].ToString(),
                        IsAdopted = reader["Status"].ToString() == "Adopted"
                    });
                }

                return Ok(cats); // Return matching cats as JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Database error: " + ex.Message);
            }
        }

        // =====================================================================
        // BreedReport METHOD
        // =====================================================================
        // WHY List<object> INSTEAD OF List<Cat>?
        // The breed report returns different columns: Breed, TotalCats, Available, Adopted
        // Our Cat class has: CatID, Name, Breed, Age, Color, IsAdopted, EntryDate
        // They DON'T match! We can't put breed report data into a Cat object.
        //
        // So we use "object" — the most flexible type in C#.
        // Every class in C# inherits from object.
        // We use "new { }" (anonymous object) to create a temporary object
        // with exactly the properties we need — without creating a separate class.
        //
        // Think of anonymous object "new { Breed=..., TotalCats=... }" as:
        // A temporary class with only these properties, created on the fly.
        // You don't need to define a full class for it.

        // =====================================================================
        // 🌟 THE ULTIMATE MASTER GUIDE: CLASSES vs. ANONYMOUS OBJECTS (new { }) 🌟
        // =====================================================================

        // ---------------------------------------------------------------------
        // 1. THE C# CLASS (The Official Government Form)
        // ---------------------------------------------------------------------
        // Think of `public class Cat` as an Official Government Form. 
        // It has perfectly printed boxes: [Name], [Breed], [Age].
        //
        // → WHY THE MAILROOM WORKER LOVES IT:
        //   The worker (Visual Studio) knows exactly how to read this form. 
        //   If the user tries to write the word "Orange" into the [Age] box, 
        //   the worker immediately spots the mistake, rejects the form at the 
        //   door, and saves your database from crashing.
        //
        // → This is incredibly safe, rigid, and predictable.

        // ---------------------------------------------------------------------
        // 2. ANONYMOUS OBJECTS (The Blank Sticky Note)
        // ---------------------------------------------------------------------
        // Code: new { Breed = "Persian", Total = 12 }
        //
        // Sometimes, the SQL Kitchen sends up a weird piece of data (like a 
        // math report) that doesn't fit into the official Cat form. 
        //
        // Instead of going through the massive legal process of printing a brand 
        // new Official Form just for this one math report, you use `new { }`.
        //
        // → `new { }` is just the worker pulling a Blank Sticky Note out of their 
        //   pocket, scribbling the math on it, and handing it to the delivery driver.
        // → It is fast, cheap, and requires no official paperwork!

        // ---------------------------------------------------------------------
        // 3. WHY NOT USE STICKY NOTES FOR EVERYTHING?! (The Wild West)
        // ---------------------------------------------------------------------
        // You asked a brilliant question: "Why not just use objects everywhere?"
        //
        // If you do this, you are turning C# into JavaScript. In JavaScript, 
        // everything is just a sticky note. Imagine trying to run a global bank 
        // where every employee just writes numbers on blank sticky notes.
        //
        // → EMPLOYEES PANIC: "Wait, is this sticky note an account number or a 
        //   phone number? Who wrote this? Where does it go?!"
        // → TYPO DISASTER: Someone accidentally spells "Depsoit" instead of 
        //   "Deposit" on their sticky note. The computer doesn't know what that 
        //   means, so the entire bank system crashes.

        // ---------------------------------------------------------------------
        // 4. THE TECHNICAL REASONS (Why C# Developers Love Classes)
        // ---------------------------------------------------------------------
        // Here is exactly how that Bank analogy applies to your C# code:
        //
        // → THE AUTOCOMPLETE PROBLEM (IntelliSense):
        //   If you use `class Cat`, typing `cat.` creates a beautiful dropdown 
        //   menu showing `.Name`, `.Age`, etc. The compiler holds your hand.
        //   If you use an anonymous `object`, Visual Studio goes completely blind. 
        //   It has no idea what is on your sticky note. You are on your own.
        //
        // → THE TYPO DISASTER (Compile-Time vs. Run-Time):
        //   If you type `cat.Nmae = "Tiger";` on an Official Form, Visual Studio 
        //   gives you a RED SQUIGGLY LINE. It physically stops you from pressing 
        //   Play until you fix it (Compile-Time Safety).
        //   If you type `myObject.Nmae` on a sticky note, Visual Studio just trusts 
        //   you. You publish the app, a user clicks it, and the SERVER CRASHES 
        //   (Run-Time Failure).
        //
        // → PERFORMANCE:
        //   Classes stack in memory perfectly like Lego bricks (lightning fast). 
        //   Sticky notes force the computer to stop and read every single note 
        //   to figure out what it says (massive lag in large apps).

        // ---------------------------------------------------------------------
        // 5. THE GOLDEN RULE OF C#
        // ---------------------------------------------------------------------
        // → 99% of the time, use Official Forms (`class Cat`). 
        //   If data is moving through your app, saving to a database, or being 
        //   passed between methods, you WANT the safety, the autocomplete, and 
        //   the red squiggly lines to protect you from typos.
        //
        // → ONLY use a Sticky Note (`new { }`) for the "Final Mile".
        //   You only use this trick at the very end of a method (like our Breed 
        //   Report) right before you shove it into the Return Envelope (JSON) 
        //   to send to the user, because you know you will never need to read it 
        //   in C# ever again.
        // =====================================================================
        [HttpGet("report")]
        public IActionResult BreedReport()
        {
            List<object> report = new List<object>();

            try
            {
                using SqlConnection con = new SqlConnection(_connectionString);
                con.Open();
                using SqlCommand cmd = new SqlCommand("sp_BreedReport", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // "new { }" creates an anonymous object with these properties
                    // No class needed — just define what you need on the spot
                    report.Add(new
                    {
                        Breed = reader["Breed"].ToString(),
                        TotalCats = Convert.ToInt32(reader["TotalCats"]),
                        Available = Convert.ToInt32(reader["Available"]),
                        Adopted = Convert.ToInt32(reader["Adopted"])
                    });
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Database error: " + ex.Message);
            }
        }
    }
}

