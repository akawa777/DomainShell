(function (definition) {
    // CommonJS
    if (typeof exports === "object") {
        module.exports = definition();

        // RequireJS
    } else if (typeof define === "function" && define.amd) {
        define(definition);

        // <script>
    } else {
        coco = definition();
    }

})(function () {// 実際の定義を行う関数
    'use strict';

    var createModel = function (model) {        
        if (typeof model == "function") {
            return $.extend({}, new model());
        } else {
            return $.extend({}, model)
        }
    }

    var register = {};

    var container = {
        set: function (name, model) {
            register[name] = model;
        },
        get: function (name) {
            var model = register[name];

            return createModel(model);
        }
    };

    var extend = function (baseModel, model) {        
        var destModel = createModel(model);
        var inheritModel = $.extend({}, baseModel, destModel);

        inheritModel.base = function () {
            var self = this;
            var base = {};

            var setFunction = function (baseModel, key) {
                return function () {
                    baseModel[key].apply(self, arguments);
                }
            }

            for (var key in baseModel) {
                if (!baseModel.hasOwnProperty(key)) {
                    continue;
                }

                if (typeof baseModel[key] == "function") {
                    base[key] = setFunction(baseModel, key);
                } else {
                    base[key] = self[key]
                }
            }

            return base;
        }

        return inheritModel;
    }

    var viewIdKey = "coco-view-id";
    var componentTag = "coco-"
    var cocoPin = "coco-pin"

    var viewId = 0;

    var getViewId = function () {
        viewId++;
        return viewId;
    }

    var coco = function (options) {
        var model;
        if (typeof options.model == "string") {            
            model = container.get(options.model);
        } else {
            model = createModel(options.model);
        }

        var node;
        if (model.node.nodeType || model.node.indexOf("#") == 0) {
            node = $(model.node).prop("outerHTML");
        } else {
            node = model.node;
        }

        var el = $(node);

        var viewId = getViewId();
        el.attr(viewIdKey, viewId);

        var context = function (selector) {
            if (!selector || selector == "") {
                return el;
            } else {
                var deleteIndexs = [];

                var jqObjs = el.find(selector).each(function (index) {
                    var root = $(this).closest("[" + viewIdKey + "]");

                    if (root.length == 0) {
                        deleteIndexs.push(index);
                    } else if (root.attr(viewIdKey) != viewId.toString()) {
                        deleteIndexs.push(index);
                    }
                });

                var deleteCount = 0;
                for (var index in deleteIndexs) {
                    var deleteIndex = deleteIndexs[index];
                    jqObjs.splice(deleteIndex - deleteCount, 1);
                    deleteCount++;
                }

                return jqObjs;
            }
        }

        context.pin = function (pin, selector) {
            if (!selector || selector == "") {
                return context("[" + cocoPin + "=" + pin + "]");                
            } else {
                return context("[" + cocoPin + "=" + pin + "]").find(selector);
            }
        }

        var params = options.params;

        model.$context = context;
        model.$params = params;
        model.$coco = coco;
        model.$components = {};

        var view = model;
        view.el = el;

        if (model.validate) {
            if (!model.validate()) {
                $(el).html("");
                return view;
            }
        }

        if (model.init) {
            model.init();
        }

        if (model.$components) {
            var components = model.$components;

            for (var key in components) {
                if (!components.hasOwnProperty(key)) {
                    continue;
                }

                var componentEl = context(componentTag + key.replace(/[A-Z]/g, function(ch){
                    return "-" + ch;
                }));

                if (componentEl.length == 0) {
                    continue;
                }

                var component = components[key];

                var componentView = coco({
                    model: component.model,
                    params: component.params
                });

                componentEl.replaceWith(componentView.el);
                component.view = componentView;
            }
        }

        model.ready();

        return view;
    }

    coco.extend = extend;
    coco.container = container;

    return coco;
});

