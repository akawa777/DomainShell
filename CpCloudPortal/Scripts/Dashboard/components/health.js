define(["text!dashboard/templates/health.html?v=3"], function (template) {
    var health = {
        template: template,
        data: {            
        },        
        init: function (adapt, options) {
            var self = this;
            adapt();
        }
    }

    return health;
});