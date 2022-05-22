# ArainUI
This is my custom UI for [TournamentAssistant](https://github.com/MatrikMoon/TournamentAssistant) by [Moon](https://github.com/MatrikMoon)

I aim to implement some advanced features while preserving cross platform compatibility for Windows, Linux, Android and Web (maybe)

*Sorry Apple device users, while Uno Platform which I chose for development has support for iOS and OSX I dont ever plan on debugging, releasing or maintaining those versions. I dont own anything apple and hate the company with a burning passion. I will not prevent it if someone comes and releases it for the Apple ecosystem, **but I will not do it myself***

## State of development
As of writing this it is not yet possible to even connect to a server, as such I will post updates here. 

You can also follow the development on the [https://trello.com/b/eEwUQwsd](Trello page) 

## Contributing
The workload to get this into working order is quite severe, **!!PRs are welcome!!**

If you ask I can add you to the trello board as well.

### Development setup
#### Prequisities
Uno platform environment and templates installed

#### Project setup
To start contributing clone this repo as well as the master branch of [Moons repo](https://github.com/MatrikMoon/TournamentAssistant), then just add the shared project from Moons repo to the soulution.

Add TAShared as a dependency to each of the heads

Unload the head projects which you are not developing for (Tizen, WPF, WPF.Host)

VisualStudio users set intellisense to UWP (This is not preserved during VS restart, keep it in mind, if you see a lot of red poop then check intellisense head)

# License
MIT
