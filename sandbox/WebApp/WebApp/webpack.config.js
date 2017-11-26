module.exports = {
    // メインとなるJavaScriptファイル（エントリーポイント）
    entry: './scripts/sample/sample.entry.ts',
    // ファイルの出力設定
    output: {
        //  出力ファイルのディレクトリ名
        path: `${__dirname}/wwwroot/js/bundles`,
        // 出力ファイル名
        filename: 'sample.bundle.js'
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                exclude: /node_modules/,
                loader: 'vue-loader'
            },
            {
                // 拡張子 .ts の場合
                test: /\.ts$/,
                exclude: /node_modules/,
                // TypeScript をコンパイルする
                use: 'ts-loader'
            },
            {
                // 拡張子 .ts の場合
                test: /\.html$/,
                exclude: /node_modules/,
                // TypeScript をコンパイルする                
                use: 'html-loader'
            }
        ]
    },
    // import 文で .ts ファイルを解決するため
    resolve: {
        extensions: [
            '.ts'
        ],
        alias: {
            'vue$': 'vue/dist/vue.esm.js'
        }
    },
    // ソースマップを有効に
    devtool: 'source-map'
};
