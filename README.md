# ArainUI
This is my custom UI for [TournamentAssistant](https://github.com/MatrikMoon/TournamentAssistant) by [Moon](https://github.com/MatrikMoon)

I aim to implement some advanced features while preserving cross platform compatibility for Windows, Linux, Android and Web (maybe)

*Sorry Apple device users, while Uno Platform which I chose for development has support for iOS and OSX I dont ever plan on debugging, releasing or maintaining those versions. I dont own anything apple and hate the company with a burning passion. I will not prevent it if someone comes and releases it for the Apple ecosystem, **but I will not do it myself***

## State of development
~~As of writing this it is not yet possible to even connect to a server, as such~~ I will post updates here. 

Update: I am starting to get busy with IRL stuff right now, so development will slow down (*not stop, just slow down*). If you have an idea please get in touch with me on discord or through the repo.

You can follow the development on the [Trello page](https://trello.com/b/eEwUQwsd)

I am also sometimes live on [Twitch](https://twitch.tv/arimodu) if you want to chat, ask or suggest some features directly to me. Or you can just come criticise my code ¯\\\_(ツ)\_/¯.

## Contributing
The workload to get this into working order is quite severe, **!!PRs are welcome!!**

If you ask I can add you to the trello board as well.

### Development setup
#### Prequisities
Uno platform environment and templates installed

#### Project setup
To start contributing clone this repo as well as the master branch of [Moons repo](https://github.com/MatrikMoon/TournamentAssistant), then just add the shared project from Moons repo to the soulution.

*Note that if you put moons repo clone into the same directory as ArainUI clone the project will be loaded automatically from the relative path in the soulution*

I recommend setup somewhat like this:
``` 
x:\xxx\SomeFolder\ArainUI\
x:\xxx\SomeFolder\TournamentAssistant\
```
If you have the project setup differently, especially so when the path
`..\TournamentAssistant\`
from the ArainUI folder is invalid (e.g. putting it to different folders) I ask that you do **not** include your soulution file in your PR

*Skip next step if relative path above works*

Add TAShared as a dependency to each of the heads

Unload the head projects which you are not developing for (Tizen, WPF, WPF.Host)

VisualStudio users set intellisense to UWP (This is not preserved during VS restart, keep it in mind, if you see a lot of red poop then check intellisense head)

#### Translating
If you want to add a translation to your language just message me on discord, I will give you access to the trello and make you a card for the translation progress

# License
MIT
