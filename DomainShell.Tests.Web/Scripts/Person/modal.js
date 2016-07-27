define(["text!/views/person/modal"], function (template) {
    var modal = {
        node: template,
        ready: function () {
            var self = this;
            
            self.$context(".modal-body").text(self.$params.body);

            self.$context(".btn-primary").on("click", function () {
                var name = self.$context("[name=name]").val();
                $(self.$params.nameTextEl).val(name);
                self.$context(".modal").modal('hide');
            });
        },
        show: function () {
            var self = this;

            self.$context(".modal").modal('show');
        }
    }

    return modal;
});