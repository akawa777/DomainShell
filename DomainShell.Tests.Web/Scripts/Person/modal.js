define(["text!/views/person/modal"], function (template) {
    var modal = {
        node: template,
        validate: function () {
            var self = this;

            if (!self.$params || !self.$params.nameTextEl) {
                console.error("there are no nameTextEl in $params");

                return false;
            }

            return true;
        },
        ready: function () {
            var self = this;

            self.$context(".btn-primary").on("click", function () {
                var name = self.$context("[name=name]").val();
                $(self.$params.nameTextEl).val(name);
                self.$context(".modal").modal("hide");
            });
        },
        show: function () {
            var self = this;

            self.$context(".modal").modal("show");
        }
    }

    return modal;
});