function CardFieldClickAddLog(id, ip) {
    $.ajax({
        url: "/Card/AddCardFieldClick",
        type: "post",
        datatype: "text",
        data: { Id: id }
    });
}

function AddNewElement(lastHTMLIndex) {
    let cardFields = document.querySelector('#cardFields');
    let cardFieldsCoun = cardFields.children.length;
    if (cardFieldsCoun >= 21) {
        alert("Максимум 21 поле")
        return;
    }
    let newCardField = document.createElement("div");
    newCardField.setAttribute("class", "cardField");
    newCardField.innerHTML = `
                                            <input class="col-sm-3 col-md-3 col-lg-3 col-xl-3" type="text" id="cardFields_`+ cardFieldsCoun + `__FieldValue" name="cardFields[` + cardFieldsCoun + `].FiledValue" value="">
                                            <label class="col-auto"> - Ссылка  </label>
                                            <input class="col-sm-3 col-md-3 col-lg-3 col-xl-3" type="text" id="cardFields_`+ cardFieldsCoun + `__FieldName" name="cardFields[` + cardFieldsCoun + `].FieldName" value="">
                                            <label class="col-auto"> - Название <a class="btn btn-outline-secondary" onclick="DeleteElement(`+ cardFieldsCoun + `,event)">Удалить</a> </label>
                                    `;
    cardFields.appendChild(newCardField);
}
function DeleteElement(index, e) {
    let field = e.currentTarget.parentNode.parentNode;
    let cardFields = document.querySelector('#cardFields');
    cardFields.removeChild(field);
    for (var i = 0; i < cardFields.childElementCount; i++) {
        cardFields.children[i].children[0].setAttribute("id", "cardFields_" + i + "__FieldValue");
        cardFields.children[i].children[0].setAttribute("name", "cardFields[" + i + "].FieldValue");
        cardFields.children[i].children[2].setAttribute("id", "cardFields_" + i + "__FieldName");
        cardFields.children[i].children[2].setAttribute("name", "cardFields[" + i + "].FieldName");
    }
}