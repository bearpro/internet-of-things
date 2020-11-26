const path = require('path');
const VueLoaderPlugin = require('vue-loader/lib/plugin');

module.exports = {
    mode: 'development',
    devtool: 'eval-source-map',
    entry: './index.js',
    output: {
        filename: 'output.js'
      },
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader',
            },
            {
              test: /\.css$/,
              use: [
                  'vue-style-loader',
                  'css-loader'
              ]
            },
        ],
    },
    plugins: [
        new VueLoaderPlugin()
    ]
}
