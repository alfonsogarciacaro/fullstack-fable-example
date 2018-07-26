# Fullstack Fable example

This project shows how Fable can be used fullstack with a node.js server written in F#. This is a variation from Maxime Mangel's [Fableconf workshop](https://github.com/fable-compiler/fableconf-workshops/tree/master/elmish) and [Fulma demo](https://github.com/MangelMaxime/fulma-demo).

## Requirements

- [Dotnet SDK 2.1.302](https://www.microsoft.com/net/download)
- [node.js with npm](https://nodejs.org)
- [Mono Framework](https://www.mono-project.com/download/stable/) for some tooling if working in non-Windows environment
- An F# IDE, like Visual Studio Code with [Ionide extension](http://ionide.io/)

## Installing dependencies

Type `npm install` to install dependencies (for both JS and F#) after cloning the repository or whenever dependencies change.

> [Paket](https://fsprojects.github.io/Paket/) is the tool used to manage F# dependencies.

## Development

Fable and Webpack are used to compile and bundle both the client and the server projects. To start them in watch mode (so the server is reloaded whenever there's a change in the code) type: `npm run start`.
