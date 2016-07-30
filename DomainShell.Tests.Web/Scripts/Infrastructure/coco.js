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
    var componentAttr = "coco-component"
    var componentTypeAttr = "coco-component-type"

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

        var node = model.node;

        var el = $(node);
        el.removeAttr(componentAttr);
        el.removeAttr(componentTypeAttr);

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
        //model.$components = {};

        var view = model;
        view.el = el;

        if (model.validate) {
            if (!model.validate()) {
                $(el).html("");
                return view;
            }
        }

        //var dfd;

        //if (model.init) {
        //    dfd = model.init();
        //}

        var applyComponent = function () {
            //if (model.$components) {
            //    var components = model.$components;

            //    for (var key in components) {
            //        if (!components.hasOwnProperty(key)) {
            //            continue;
            //        }

            //        var componentEl = context("[" + componentAttr + "=" + key + "]");

            //        if (componentEl.length == 0) {
            //            continue;
            //        }

            //        var component = components[key];

            //        if (!component.model.node) {
            //            component.model.node = componentEl.prop("outerHTML");
            //        }

            //        var createFakeBaseEl = function (el) {
            //            var baseEl = $("<script></script>");
            //            el.replaceWith(baseEl)

            //            return baseEl;
            //        }

            //        var baseEl = createFakeBaseEl(componentEl);

            //        if (componentEl.attr(componentTypeAttr) == "each") {                        
            //            var eachCoco = function (eachParams) {
            //                var prevEl;
            //                var views = [];

            //                if (eachParams.length == 0) {
                                
            //                } else {
            //                    eachParams.forEach(function (params) {
            //                        var view = coco({
            //                            model: component.model,
            //                            params: params
            //                        });

            //                        if (prevEl) {
            //                            prevEl.after(view.el);
            //                            prevEl = view.el;
            //                        } else {
            //                            baseEl.replaceWith(view.el);
            //                            baseEl = view.el;
            //                            prevEl = view.el;
            //                        }

            //                        views.push(view);
            //                    });
            //                }                            

            //                component.views = views;

            //                component.views.append = function (params, index) {
            //                    if (index == null) {
            //                        index = component.views.length - 1;
            //                    }

            //                    var targetEl = component.views.length == 0 ? baseEl : component.views[index].el;

            //                    var view = coco({
            //                        model: component.model,
            //                        params: params
            //                    });

            //                    if (component.views.length == 0) {
            //                        targetEl.replaceWith(view.el);
            //                    } else {
            //                        targetEl.after(view.el);
            //                    }

            //                    if (index == component.views.length - 1) {
            //                        component.views.push(view);
            //                    } else {
            //                        component.views.splice(index + 1, 0, view);
            //                    }
            //                }

            //                component.views.prepend = function (params, index) {
            //                    if (index == null) {
            //                        index = 0;
            //                    }

            //                    var targetEl = component.views.length == 0 ? baseEl : component.views[index].el;

            //                    var view = coco({
            //                        model: component.model,
            //                        params: params
            //                    });

            //                    if (component.views.length == 0) {
            //                        targetEl.replaceWith(view.el);
            //                    } else {
            //                        targetEl.before(view.el);
            //                    }

            //                    if (index == 0) {
            //                        component.views.unshift(view);
            //                    } else {
            //                        component.views.splice(index, 0, view);
            //                    }
            //                }

            //                component.views.remove = function (index) {
            //                    if (component.views.length == 0) {
            //                        return;
            //                    }

            //                    if (index == null) {
            //                        index = component.views.length - 1;
            //                    }

            //                    if (component.views.length == 1) {                                    
            //                        baseEl = createFakeBaseEl(component.views[index].el);
            //                    } else {
            //                        component.views[index].el.remove();
            //                    }

            //                    component.views.splice(index, 1);
            //                }

            //                component.views.removeAll = function () {
            //                    if (component.views.length == 0) {
            //                        return;
            //                    }

            //                    for (var index = 0; index < component.views.length; index++) {
            //                        if (index == 0) {
            //                            baseEl = createFakeBaseEl(component.views[0].el);
            //                        }

            //                        component.views[index].el.remove();
            //                    }

            //                    component.views.length = 0;
            //                }
            //            }

            //            eachCoco(component.eachParams, componentEl);

            //            component.reload = function (eachParams) {
            //                for (var index = 0; index < component.views.length; index++) {
            //                    if (index == 0) {
            //                        baseEl = createFakeBaseEl(component.views[0].el);
            //                    }

            //                    component.views[index].el.remove();
            //                }

            //                component.views.length = 0;

            //                eachCoco(eachParams, baseEl);
            //            }
            //        } else {
            //            var singleCoco = function (params, firstEl) {
            //                var view = coco({
            //                    model: component.model,
            //                    params: params
            //                });

            //                firstEl.replaceWith(view.el);
            //                component.view = view;
            //            }

            //            singleCoco(component.params, componentEl);

            //            component.reload = function (params) {
            //                singleCoco(params, component.view.el);
            //            }
            //        }
            //    }
            //}
        }

        //if (dfd && dfd.done) {
        //    dfd.done(function () {
        //        applyComponent();
        //        if (model.ready) {
        //            model.ready();
        //        }                
        //    });

        //} else {
        //    applyComponent();
        //    if (model.ready) {
        //        model.ready();
        //    }
        //}

        if (model.ready) {
            model.ready();
        }

        return view;
    }

    coco.extend = extend;
    coco.container = container;

    return coco;
});

