

#region Classic boilerplate code.

// using System;

// namespace MyShopOnLine.Api.WhatsNew
// {
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("Hello World!");
//        }
//    }
// }

#endregion


// The compiler allows us to omit the noisy boilerplate of Program class.
#region Top-Level programs - step 1

// System.Console.WriteLine("Hello World, by Top Level Statements");

#endregion

#region A Web API using just a line of code - step 2

// using Microsoft.AspNetCore;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;


// WebHost.CreateDefaultBuilder().Configure(app => app.Run(d => d.Response.WriteAsync("Exposing a Web Api with just a line of code."))).Build().Run();

#endregion


#region A Web API using just a line of code - step 3

// using Microsoft.AspNetCore;
// using Microsoft.AspNetCore.Builder;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;

// WebHost.CreateDefaultBuilder()
//    .ConfigureServices(services =>
//    {
//        // Dependency injection and ASP.NET Core services.
//    }).Configure(app =>
//        app.Run(d => d.Response.WriteAsync("Exposing a Web Api with just a line of code.")
//        ))
//    .Build().Run();

#endregion


#region A Concrete example - step 4

using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


WebHost.CreateDefaultBuilder().
    ConfigureServices(s => s.AddSingleton<StudentService>()).
    Configure(app =>
    {
        app.UseRouting();
        app.UseEndpoints(e =>
        {
            var studentService = e.ServiceProvider.GetRequiredService<StudentService>();

            e.MapGet("/students",
                async c => await c.Response.WriteAsJsonAsync(await studentService.GetAll()));
            e.MapGet("/students/{id:int}",
                async c => await c.Response.WriteAsJsonAsync(await studentService.Get(int.Parse((string)c.Request.RouteValues["id"]))));
            e.MapPost("/students",
                async c =>
                {
                    await studentService.Add(await c.Request.ReadFromJsonAsync<Student>());
                    c.Response.StatusCode = 201;
                });
            e.MapDelete("/students/{id:int}",
                async c =>
                {
                    await studentService.Delete(int.Parse((string)c.Request.RouteValues["id"]));
                    c.Response.StatusCode = 204;
                });
        });
    }).Build().Run();

    public record Student(int Id, string Name, string Surname, string UniversityName);

    public class StudentService
    {
        private readonly List<Student> students = new List<Student>
        {
                new Student(1,"Pietro","Libro","University of Rome, 'La Sapienza'"),
                new Student(2,"Pinco","Pallo","ETH"),
                new Student(3, "John", "Smith", "Universität Zürich")
        };

        public Task<IEnumerable<Student>> GetAll() => Task.FromResult(students.AsEnumerable());

        public Task<Student> Get(int id) => Task.FromResult(students.FirstOrDefault(x => x.Id == id));

        public Task<int> Add(Student newStudent)
        {
            students.Add(newStudent with { Id = (students.LastOrDefault()?.Id ?? 0) + 1 });

            return Task.FromResult(newStudent.Id);
        }

        public async Task Delete(int id)
        {
            var Student = await Get(id);
            if (Student == null)
            {
                throw new InvalidOperationException(string.Format("Student with id '{0}' doesn't exists", id));
            }

            students.Remove(Student);
        }
    }

#endregion