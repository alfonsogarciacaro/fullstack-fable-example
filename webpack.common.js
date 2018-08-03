var apiServerPort = 9001;
var devServerPort = 9000;

module.exports = {
    indexHtmlTemplate: "./src/index.html",
    serverEntry: "./src/Server/Server.fsproj",
    clientEntry: "./src/Client/Client.fsproj",
    cssEntry: "./src/style/main.scss",
    outputDir: "./deploy",
    assetsDir: "./public",
    serverPort: apiServerPort,
    devServerPort: devServerPort,
    devServerProxy: {
        '/api/*': {
            target: "http://localhost:" + apiServerPort,
        }
    },
    babel: {
        presets: [["env", { "modules": false }]],
    }
};
