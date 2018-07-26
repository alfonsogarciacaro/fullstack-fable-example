var path = require("path");
var NodemonPlugin = require('nodemon-webpack-plugin');
var CONFIG = require("./webpack.common.js");

// If we're running in watch mode, assume we're in development
var isProduction = !process.argv.find(v => v === "-w" || v === "--watch");
console.log("Bundling SERVER for " + (isProduction ? "production" : "development") + "...");

module.exports = {
    entry: CONFIG.serverEntry,
    mode: isProduction ? "production" : "development",
    devtool: isProduction ? "source-map" : "eval-source-map",
    output: {
        path: path.join(__dirname, CONFIG.outputDir),
        filename: "server.js"
    },
    target: "node",
    plugins: isProduction ? [] : [new NodemonPlugin({
        args: ["--port", String(CONFIG.serverPort)]
    })],
    module: {
        rules: [
            {
                test: /\.fs(x|proj)?$/,
                use: "fable-loader"
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                    options: CONFIG.babel
                },
            },
        ]
    }
};
