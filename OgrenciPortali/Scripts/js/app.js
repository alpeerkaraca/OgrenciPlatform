function showFeedbackModal(status, title, message, confirmCallback, confirmButtonText, onCloseCallback) {
    var modal = $('#feedbackModal');
    var header = $('#feedbackModalHeader');
    var icon = $('#feedbackModalIcon');
    var modalTitle = $('#feedbackModalTitle');
    var body = $('#feedbackModalBody');
    var confirmBtn = $('#feedbackConfirmBtn');
    var closeBtn = modal.find('.modal-footer .btn-secondary');

    modal.off('hidden.bs.modal');

    header.removeClass('bg-success bg-danger bg-warning bg-info');
    confirmBtn.removeClass('btn-success btn-danger btn-warning btn-info');
    icon.removeClass();

    switch (status) {
    case 'success':
        header.addClass('bg-success');
        icon.addClass('bi bi-check-circle-fill');
        break;
    case 'error':
        header.addClass('bg-danger');
        icon.addClass('bi bi-x-circle-fill');
        break;
    case 'warning':
        header.addClass('bg-warning');
        icon.addClass('bi bi-exclamation-triangle-fill');
        break;
    case 'info':
        header.addClass('bg-info');
        icon.addClass('bi bi-info-circle-fill');
        break;
    case 'confirm':
        header.addClass('bg-danger');
        icon.addClass('bi bi-exclamation-triangle-fill');
        break;
    default:
        header.addClass('bg-secondary');
        icon.addClass('bi bi-question-circle-fill');
        break;
    }

    modalTitle.text(title);
    body.html(message);

    if (status === 'confirm' && typeof confirmCallback === 'function') {
        const originalButtonText = confirmButtonText || 'Onayla';
        confirmBtn.text(originalButtonText);
        confirmBtn.addClass('btn-danger');
        confirmBtn.show();
        confirmBtn.prop('disabled', false);
        closeBtn.prop('disabled', false);

        confirmBtn.off('click').on('click',
            async function() {
                confirmBtn.prop('disabled', true);
                closeBtn.prop('disabled', true);
                confirmBtn.html('<span class="spinner-border spinner-border-sm"></span> Ýsleniyor...');

                try {
                    await confirmCallback();
                } catch (err) {
                    console.error("Callback hatasý:", err);
                } finally {
                    modal.modal('hide');
                    modal.one('hidden.bs.modal',
                        function() {
                            confirmBtn.prop('disabled', false);
                            closeBtn.prop('disabled', false);
                            confirmBtn.html(originalButtonText);
                        });
                }
            });
    } else {
        confirmBtn.hide();
    }

    modal.modal('show');

    if (typeof onCloseCallback === 'function') {
        modal.one('hidden.bs.modal',
            function() {
                onCloseCallback();
            });
    }
}