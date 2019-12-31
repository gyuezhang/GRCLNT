using System;
using Stylet;
using StyletIoC;
using GRCLNT;

namespace GRCLNT
{
    public class Bootstrapper : Bootstrapper<WndLoginViewModel>
    {
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
        }
    }
}
