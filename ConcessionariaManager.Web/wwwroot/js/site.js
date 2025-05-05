

$(document).ready(function () {
    $("#telefone").inputmask({
        mask: ['(99) 9999-9999', '(99) 99999-9999'],
        onBeforePaste: function (pastedValue, opts) {
            var cleaned = pastedValue.replace(/\D/g, '');
            return cleaned.length === 10 ? "(99) 9999-9999" : "(99) 99999-9999";
        },
        onKeyValidation: function (key, result) {
            if (result) {
                var value = $("#telefone").val();
                if (value.indexOf('_') !== -1) {
                    $("#telefone").val(value.replace('_', ''));
                }
            }
        }
    });
});

$("#telefone").on('input', function () {
    var telefone = $(this).val().replace(/\D/g, '');
    var telefoneValido = /^[0-9]{10,11}$/.test(telefone);

    if (!telefoneValido) {
        $(this).next('.text-danger').text('Número de telefone inválido.');
    } else {
        $(this).next('.text-danger').text('');
    }
});

$("#cep").inputmask('99999-999');
function preencherEnderecoPorCep(cep) {
    fetch('https://viacep.com.br/ws/' + cep + '/json/')
        .then(response => response.json())
        .then(data => {
            if (data.erro) {
                alert('CEP não encontrado.');
                return;
            }
            document.getElementById('logradouro').value = data.logradouro || '';
            document.getElementById('bairro').value = data.bairro || '';
            document.getElementById('localidade').value = data.localidade || '';
            document.getElementById('uf').value = data.uf || '';

            document.getElementById('logradouro_hidden').value = data.logradouro || '';
            document.getElementById('bairro_hidden').value = data.bairro || '';
            document.getElementById('localidade_hidden').value = data.localidade || '';
            document.getElementById('uf_hidden').value = data.uf || '';
        })
        .catch(error => {
            console.error('Erro ao buscar o CEP: ', error);
            alert('Erro ao buscar o CEP.');
        });
}

$("#cep").on('input', function () {
    var cep = this.value.replace(/\D/g, '');
    if (cep.length === 8) {
        preencherEnderecoPorCep(cep);
    }
});

var cepInicial = $("#cep").val()?.replace(/\D/g, '');
if (cepInicial?.length === 8) {
    preencherEnderecoPorCep(cepInicial);
};
