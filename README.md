[![NuGet Version](http://img.shields.io/nuget/v/GeekLearning.Email.svg?style=flat-square&label=NuGet)](https://www.nuget.org/packages/GeekLearning.Email/)
[![Build Status](https://geeklearning.visualstudio.com/_apis/public/build/definitions/f841b266-7595-4d01-9ee1-4864cf65aa73/28/badge)](#)

# gl-dotnet-email

GeekLearning.Email provide an abstraction over various email providers. It brings builtin templates
support thanks to our [templating library](https://github.com/geeklearningio/gl-dotnet-templating). 
It also bring email interception support so you can easily redirect email to developers/tester 
inboxes in developement environement. 

## Getting Started


In your project.json add required dependencies :
```
"GeekLearning.Storage.FileSystem": "0.6.0-*",
"GeekLearning.Templating.Handlebars": "0.5.0-*",

"GeekLearning.Email": "0.5.0-*",
"GeekLearning.Email.Smtp": ""0.5.0-*"
```

Then add required settings in your `appsettings.json` file. 

In this example, we will use FileSystem provider to configure a storage provider which will load files from
a `Templates` folder relative to Application Root. This could be configured to use 
an Azure Container instead (see storage documentation).

We will configure `Email` to use the `Smtp` provider. If any Mockup Recipients are defined, they will
receive the email in place of the original Recipients. 

```json
"Email": {
    "Provider": {
      "Type": "Smtp",
      "Parameters": {
        "Host": "127.0.0.1",
        "Port": "25",
        "UserName": "",
        "Password": ""
      },
    },
    "DefaultSender": {
      "Email": "no-reply@yourdomain.com",
      "DisplayName": "Your Company Name"
    },
    "TemplateStorage": "Templates",
    "Mockup": {
      "Recipients": [],
      "Exceptions": {
        "Emails": [],
        "Domains": []
      }
    }
  },
"Storage": {
"Stores": {
    "Templates": {
    "Provider": "FileSystem",
    "Parameters": {
        "Path": "Templates"
    }
    }
}
}
```

Then in your `Startup.cs` file add required dependencies and configuration to the DI container. 

```csharp
services.AddStorage().AddFileSystemStorage(this.HostingEnvironment.ContentRootPath);
services.Configure<StorageOptions>(Configuration.GetSection("Storage"));
services.AddTemplating().AddHandlebars();

services.AddEmail()
    .AddSmtpEmail();
services.Configure<EmailOptions>(Configuration.GetSection("Email"));
```

Then we will have to write our first email template. The library uses a suffix convention to name
subtemplate needed to generate subject, html and text versions. For instance, if we want to 
define an `Invitation` template, we will write three templates :
* Invitation-BodyHtml.hbs
* Invitation-BodyText.hbs
* Invitation-Subject.hbs

*Check the sample project to see the template contents*

Then in our classes, we can require an `IEmailSender` which will allow us to send templated
emails.

```csharp
 public class HomeController : Controller
    {
        private IEmailSender emailSender;

        public HomeController(IEmailSender emailSender)
        {
             this.emailSender = emailSender;
        }

        public async Task<IActionResult> SendEmail()
        {
            var user = new User
            {
                Email = "john@doe.me",
                DisplayName = "John Doe"
            };

            var context = new
            {
                ApplicationName = "Email Sender Sample",
                User = user
            };

            await this.emailSender.SendTemplatedEmailAsync("Invitation", context, user);

            return RedirectToAction("Index");
        }
    }
```

## Supported providers

We currently support two providers in addition to the testing oriented `InMemoryProvider`.

### Smtp

Thanks to mailkit library, you can send email using any smtp endpoint.

### SendGrid

We also bring basic sendgrid api support using our Sendgrid plugin.