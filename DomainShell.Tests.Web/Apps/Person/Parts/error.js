define(["coco"], function (coco) {
    var error = {
        node: '<span class="valid" style="color:red;"></span>',
        ready: function () {
            var self = this;
            self.$context().hide();
        },
        message: function (message) {
            var self = this;
            self.$context().text("*" + message);
            self.$context().show();
        },
        clear: function () {
            var self = this;
            self.$context().hide();
        }
    };

    return error
});