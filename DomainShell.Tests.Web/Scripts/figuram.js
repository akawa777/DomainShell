(function (definition) {
    // CommonJS
    if (typeof exports === "object") {
        module.exports = definition();

        // RequireJS
    } else if (typeof define === "function" && define.amd) {
        define(definition);

        // <script>
    } else {
        figuram = definition();
    }

})(function () {// 実際の定義を行う関数
    'use strict';

    var figuram = function (options) {
        var context = function (selector) {
            if (selector) {
                return $(options.el).find(selector);
            } else {
                return $(options.el);
            }
        }

        var app = options.app;

        if (app.template) {
            $(options.el).html(app.template);
        }

        var model = app.ready(context, options.parameters, figuram);

        if (app.components) {
            var components = app.components;

            for (var key in components) {
                if (!components.hasOwnProperty(key)) {
                    continue;
                }

                var el = context("component-" + key);
                el.css("display", "inline");

                var parameters = components[key].parameters;

                var model = figuram({
                    el: el,
                    app: components[key].app,
                    parameters: components[key].parameters
                });

                components[key].model = model;
            }
        }

        return model;
    }

    return figuram;
});

