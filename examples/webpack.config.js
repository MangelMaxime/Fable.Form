var path = require("path");
var webpack = require("webpack");

// If we're running the webpack-dev-server, assume we're in development mode
const isDevelopment = process.env.NODE_ENV !== 'production';
console.log("Bundling for " + (isDevelopment ? "development" : "production") + "...");

/** @type {import("webpack").Configuration } */
module.exports = {
    entry: './fableBuild/src/App.js',
    output: {
        path: path.join(__dirname, "dist"),
    },
    mode: isDevelopment ? 'development' : 'production',
    devServer: {
        hot: true,
        contentBase: path.join(__dirname, 'public'),
        publicPath: "/",
        proxy: {
            '/api/*': {
                target: 'https://localhost:3000',
                changeOrigin: true,
                secure: false
            }
        }
    },
    module: {
        rules: [
            {
                test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*)?$/,
                use: ["file-loader"]
            }
        ],
    },
    plugins: [
        // ... other plugins
        // isDevelopment && new ReactRefreshWebpackPlugin(),
    ].filter(Boolean),
    // ... other configuration options
};
