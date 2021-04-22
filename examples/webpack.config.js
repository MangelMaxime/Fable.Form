const path = require("path")
const webpack = require("webpack")
const HtmlWebpackPlugin = require('html-webpack-plugin')
const MiniCssExtractPlugin = require('mini-css-extract-plugin')

function resolve(filePath) {
    return path.join(__dirname, filePath)
}

module.exports = (_env, options) => {

    var isDevelopment = options.mode === "development";

    return {
        entry: './fableBuild/src/App.js',
        mode: isDevelopment ? "development" : "production",
        output: {
            path: resolve('./output'),
            filename: isDevelopment ? '[name].js' : '[name].[fullhash].js',
        },
        devtool: isDevelopment ? 'eval-source-map' : null,
        optimization: {
            // Split the code coming from npm packages into a different file.
            // 3rd party dependencies change less often, let the browser cache them.
            splitChunks: {
                cacheGroups: {
                    commons: {
                        test: /node_modules/,
                        name: "vendors",
                        chunks: "all"
                    }
                }
            },
        },
        plugins:
            [
                new HtmlWebpackPlugin({
                    filename: "./index.html",
                    template: "./src/index.html"
                }),
                new MiniCssExtractPlugin()
            ].filter(Boolean),
        devServer: {
            contentBase: resolve("public"),
            publicPath: "/",
            port: 8080,
            hot: true,
            inline: true
        },
        module: {
            rules: [
                {
                    test: /\.(sass|scss|css)$/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        'css-loader',
                        'sass-loader',
                    ],
                },
                {
                    test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)$/,
                    use: ["file-loader"]
                }
            ]
        }
    }
}
