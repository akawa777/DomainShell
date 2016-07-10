(function (definition) {
    // CommonJS
    if (typeof exports === "object") {
        module.exports = definition();

        // RequireJS
    } else if (typeof define === "function" && define.amd) {
        define(definition);

        // <script>
    } else {
        view = definition();
    }

})(function () {// 実際の定義を行う関数
    'use strict';

    var view = function (options) {
        var context = function (selector) {
            return $(options.el).find(selector);
        }

        new options.app(context, options.parameters);
    }

    return view;
});