define(["coco"], function (coco) {
    var error = {
        node: '<span class="valid" style="color:red;"></span>',
        ready: function () {
            var self = this;
            self.$context().hide();
        },
        verify: function () {
            var self = this;
            
            var message = function (message) {                
                self.$context().text("*" + message);
                self.$context().show();
            }

            return self.$params.verify(message);
        },
        clear: function () {
            var self = this;
            self.$context().hide();
        }
    };

    return error
});