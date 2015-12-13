define(["text!shared/templates/notice.html"], function (template) {
    var notice = {        
        template: template,
        data: {            
        },
        modal: function (args) {

        },
        editHook: function (el) {
            var self = this;

            self.modal = function (args) {
                if (args) {
                    $(el).modal(args);
                } else {
                    $(el).modal(args);
                }
            }
        },        
        clickNewCreate: function (el) {
            var self = this;
            self.modal();
        },
        clickEdit: function (el) {
            var self = this;
            self.modal();
        },
        init: function (adapt) {
            adapt();
        }
    }    

    return notice;
});