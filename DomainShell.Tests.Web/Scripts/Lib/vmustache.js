define(["Mustache"], function (Mustache) {
    var randobet = function (n, b) {
        b = b || '';
        var a = 'abcdefghijklmnopqrstuvwxyz'
            + 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
            + '0123456789'
            + b;
        a = a.split('');
        var s = '';
        for (var i = 0; i < n; i++) {
            s += a[Math.floor(Math.random() * a.length)];
        }
        return s;
    };

    function Vmustache() {
        this.stateData = {
            template: "",
            refKeyMap : {},
            cacheActions: [],
            data: {}
        };        
    }

    Vmustache.prototype.setEvent = function (action) {
        var self = this;
        var func = function () {
            self.stateData.cacheActions.push({ item: this, action: action });
            return "cacheActions." + (self.stateData.cacheActions.length - 1).toString();
        }

        return func;
    };

    Vmustache.prototype.getRefKey = function (groupKey) {
        var self = this;

        if (groupKey) {
            if (self.stateData.refKeyMap[groupKey]) {
                return self.stateData.refKeyMap[groupKey];
            }
            else
            {
                var key = "refkey_" + groupKey + "_" + randobet(5);
                self.stateData.refKeyMap[groupKey] = key;

                return key;
            }
        }

        return "refkey_" + randobet(5);
    };

    Vmustache.prototype.render = function (template, data) {
        var self = this;
        self.stateData.template = template;
        self.stateData.data = data;
        var html = Mustache.render(template, self.stateData.data);

        return html;
    };

    Vmustache.prototype.rerender = function () {
        var self = this;
        var html = Mustache.render(self.stateData.template, self.stateData.data);

        return html;
    };

    return Vmustache;
});