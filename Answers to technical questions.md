 #### 1. How long did you spend on the coding assignment? What would you add to your solution if you had more time? If you didn't spend much time on the coding assignment then use this as an opportunity to explain what you would add.

>  It took 4 hours in analyzing and understanding the APIs (Exchange Rate Providers), and around 30+ hours in coding.
If I have more time I would rather use AWS to consume the distributed Cache(ElasticCache) instead of the MemoryCache, besides, I would use AWS Parameter Store to save Coinmarketcap's API key and JWT Encryption key, hence, loading them from there on run time instead of adding them to the appsettings.config (which is not acceptable in real life app for sure), also I could add API rate limit to the created API as a kind of protection and security.

 
 #### 2.  What was the most useful feature that was added to the latest version of your language of choice? Please include a snippet of code that shows how you've used it

> In this assessment I used .net core 3.1, I would like to mention what I found great in .net core compared with .net framework, the most impressive thing is definitely the built-in dependency injection which makes our life as developers really easy where we used to use some frameworks like Ninject for DI. and what I feel is great is how we can easily interfere in the request life cycle to add/change the behaviour by creating middleware, which I used for error handling and for authentication in the assessment, not to mention adding  HttpClientFactory in .net core to facilitate dealing with HttpClient to specific API and giving an ability to applying “Exponential backoff retry pattern” via HttpPolicyHandler in few line of code
> 
> **Snippet of code**
> - Dependency Injection:

    ` public void ConfigureServices(IServiceCollection services)
            {
                services.AddControllers();
                services.AddHttpClient();
                services.AddOptions();
                services.AddHealthChecks();
                services.AddSingleton<IConfiguration>(Configuration);
                services.AddMemoryCache();
                services.AddOptions();
                services.InjectCryptocurrencyProviderService(Configuration);
                services.InjectSwaggerServices(API_NAME, Configuration);
                services.AddSingleton<IUserService, StaticUserService>();
                services.InjectJWTService(Configuration);
                services.InjectRecurringJobService();
            }`

> - Middleware:`

    public class JwtMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly JWTSettings _jwtSettings;
            private readonly ILogger<JwtMiddleware> _logger;
            public JwtMiddleware(RequestDelegate next, IOptions<JWTSettings> jwtSettings, ILogger<JwtMiddleware> logger)
                {
                    _next = next;
                    _jwtSettings = jwtSettings.Value;
                    _logger = logger;
                }
        
         public async Task Invoke(HttpContext context)
                {
                    var splits = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ');
                    if(splits!=null && splits.Length==2 && splits[0].ToLower()=="bearer" && splits[1] != null)
                        AttachUserToContext(context, splits[1]);
        
                    await _next(context);
                }
        
                private void AttachUserToContext(HttpContext context, string token)
                {
                    try
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
                        tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ClockSkew = TimeSpan.Zero
                        }, out SecurityToken validatedToken);
        
                        var jwtToken = (JwtSecurityToken)validatedToken;
                        var UserName = jwtToken.Claims.First(x => x.Type == "UserName").Value;
                        var userpermissions = jwtToken.Claims.First(x => x.Type == "permissions").Value;
                        var permissions = new List<string>();
                        if (!string.IsNullOrEmpty(userpermissions))
                            permissions.AddRange(userpermissions.Split(','));
                        context.Items["UserClient"] = new UserClient() { UserName = UserName, Permissions = permissions };
                    }
                    catch
                    {
                        _logger.LogInformation("unauthorized attempt to call the api");
                    }
                }
            }`

> - HttpClient injection with Policy

    public static class ExchangeratesAPIProviderServiceInjectionExtension
        {
            public static void InjectExchangeratesAPIProviderService(this IServiceCollection services, IConfiguration configuration)
            {
                
                string serviceurl=configuration.GetValue<string>("exchangeratesapi.io:ServiceBaseUrl");
    
     services.AddHttpClient("exchangeratesapi.io", c =>
                {
                    c.BaseAddress = new Uri(serviceurl);
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                    .AddPolicyHandler(GetRetryPolicy());
    
            }
            static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            {
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(4,retryAttempt => TimeSpan.FromSeconds(retryAttempt*2));
            }
    
        }

#### 3. How would you track down a performance issue in production? Have you ever had to do this?

>The most important thing is writing logs to track the errors and the performance-related issues, without logs the app in production will turn to a black box, these logs should be searchable easily so we need to use a searching and monitoring software like Splunk that facilitates searching within the big text (such as IIS logs, event logs, s3 logs .. etc) and give the ability to create alerts.
Another tool which it is really good to have is “SpeedCurve” that monitors the performance of the websites and takes snapshots from the page while loading every few second so we can know when the page took a long time to load then we can search in (Splunk, DB logs .. etc) to know what the root cause of the issue in that specific time is.
 Heartbeat tools (such as DataDog) also are very helpful to get an immediate notification once one of the services or sites in the cloud is not responding.
Finally, AWS and other cloud services providers also provide great services for monitoring (Such as CloudWatch) that can trigger an alarm, for example, if the CPU exceeds a specific threshold so we can check the log of the apps and DBs to find the root cause. 
In short, we should know when/where the log should be written a log in the app and use the correct tools to search and analyze these logs, and using correct tools to monitor the health of the apps and the infrastructure
In my current company “Monster.com”, we encounter performance issues every now and then due to the very heavy traffic, In the last application my team worked on was Monster Salary app “Monster.com/Salary” which is .net core 3.1 with React frontend, we faced a spike in the SpeadCurve and received an alert that the page takes long time to respond, we checked the application logs and found that all the request we received in that time have been serviced from "Salary provider API" instead of ElasticCache, so we checked the cache in that period and found the cache was full due to very heavy and non-unique requests so the cache was not responding, we fixed that by upgrading ElasticCache instance type and add one extra cache instance
 


 

#### 4. What was the latest technical book you have read or tech conference you have been to? What did you learn?

>  I read a few chapters from “Practical Text Mining and Statistical Analysis for Non-structured Text Data Applications”, although it is almost 10 years old, it is a very helpful book to understand the text mining related fields such as (Information Retrieval, Clustering, Document Classifications, Web Mining, Information extraction, NLP and Concept extraction), I learned how to identify the text mining field we need, based on the text resources and project goals

 #### 5. What do you think about this technical assessment?
 
> I like the assessment, it gave the opportunity to show some technical
 skills in designing the solution as well as OOP, design patterns, DI,  async tasks and .ne core, it requires conducting the integration with two different currency exchange providers (one for Cryptocurrencies and another for fiat currencies), so the challenge was to build it in a dynamic way to consolidate the response from multiple currency exchange providers responses, and this is what I have done, as the design accept adding a new provider and plug it to the application easily, besides, in case we have the paid version of Coinmarketcap API, we can tell the application to use only that provider to give the needed info (exchange rate of the cryptocurrency in multiple fiat  currencies) without touching the code, we need only to change only one attribute in the appsetting.config file to achieve that.
In general, the API requires few minor changes like using distributed cache instead of MemoryCache, and moving some secret keys from the config to any cloud vault, such as AWS secrets manager then the API would be ready for deploying to production and handle high traffic and respect the providers' service-level agreement (SLA).
The API has been secured using JWT authentication (bearer token), and I used HangFire for recurring jobs and Swager for testing

 #### 6. Please, describe yourself using JSON.

    {
           "name":"Mouayad Khashfeh",
           "birthDate":"1986-05-12T00:00:00.000Z",
           "about":"Senior Software Engineer (14 years of experience in Software Engineering) with a history of productivity and successful project outcomes.\n Currently, I'm playing a team lead role in Monster Worldwide (Kuala Lumpur Center).\nI have a Master in Information Technology 2020, my thesis focused on Text mining and Distributed Systems.\n Throughout my career, I have demonstrated the highest levels of service and commitment to the mission of any organization I have worked for. In my current company (Monster) I got Monster Spot Award 2020, and Monster High Achiever Award 2019",
           "main_skills":[
              "Software Engineering",
              "Solution Architect",
              "Microservices",
              "Cloud Computing",
              "Data Science",
              ".Net Core",
              "C#",
              "Sql/T-Sql",
              "MVC",
              "AWS",
              "Core Banking Systems",
              "TDD",
              "Agile"
           ],
           "education":{
              "degree":"Master of Information Technology",
              "school":"Universiti Tenaga Nasional",
              "field_of_study":"Data Science",
              "end_date":"2020-10-22T00:00:00.000Z"
           },
           "latest_job":{
              "company":"Monster Worldwide",
              "position":"Team Lead / Sr. Software Engineer",
              "start_date":"2017-04-01T00:00:00.000Z",
              "finish_date":null,
              "is_current":true,
              "location":{
                 "country":"Malaysia",
                 "city":"Kuala Lumpur"
              },
              "tasks":"- Team Lead of \"Guardians\" Team, consisting of 11 members (Developers and QAs) with 70% involvement in coding.\n- Architect new products from scratch, based on the organization vision to achieve its targets in the market, besides, maintaining and improving the existing products\n- Ensuring delivering product with high-quality code in terms of (scalability, maintainability, understandability), besides, conducting the required testing (Unit test, Automation test, Regression test, Stress test) with all required documentation (Client Guide, Developer Guide, QA Guide (Test Cases))\n- Create and maintain technical documentation, and contribute to project deliverables where applicable\n- Participate in peer-reviews of solution designs and related code\n- Package and support deployment of releases",
              "technologies":[
                 "C#",
                 "MVC.net",
                 ".Net Core",
                 "SQL Server",
                 "AWS Cloud",
                 "Elastic Search",
                 "Apache Kafka",
                 "React",
                 "Jenkins",
                 "MySql",
                 "Memcached"
              ]
           }
        }
