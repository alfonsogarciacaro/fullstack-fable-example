var path = require("path");
var webpack = require("webpack");
var NodemonPlugin = require('nodemon-webpack-plugin');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');
var MiniCssExtractPlugin = require("mini-css-extract-plugin");
var CONFIG = require("./webpack.common.js");

// If we're running webpack-dev-server, assume we're in development
var isProduction = !process.argv.find(v => v.indexOf('webpack-dev-server') !== -1);
console.log("Bundling CLIENT for " + (isProduction ? "production" : "development") + "...");

// The HtmlWebpackPlugin allows us to use a template for the index.html page
// and automatically injects <script> or <link> tags for generated bundles.
var clientPlugins = [
    new HtmlWebpackPlugin({
        filename: 'index.html',
        template: CONFIG.indexHtmlTemplate
    })
];

module.exports = {
    entry: isProduction ? {
        client: [CONFIG.clientEntry, CONFIG.cssEntry]
    } : {
            client: [CONFIG.clientEntry],
            style: [CONFIG.cssEntry]
        },
    output: {
        path: path.join(__dirname, CONFIG.outputDir, path.basename(CONFIG.assetsDir)),
        filename: isProduction ? '[name].[hash].js' : '[name].js',
    },
    mode: isProduction ? "production" : "development",
    devtool: isProduction ? "source-map" : "eval-source-map",
    plugins: isProduction ?
        clientPlugins.concat([
            new MiniCssExtractPlugin({ filename: 'style.css' }),
            new CopyWebpackPlugin([{ from: CONFIG.assetsDir }]),
        ])
        : clientPlugins.concat([
            new webpack.HotModuleReplacementPlugin(),
        ]),
    devServer: {
        publicPath: "/",
        contentBase: CONFIG.assetsDir,
        port: CONFIG.devServerPort,
        proxy: CONFIG.devServerProxy,
        hot: true,
        inline: true
    },
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
            {
                test: /\.(sass|scss|css)$/,
                use: [
                    "style-loader",
                    "css-loader",
                    "sass-loader"
                ]
            },
            {
                test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*$|$)/,
                use: ["file-loader"]
            }
        ]
    }
};
