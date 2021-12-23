import React, {memo, useEffect, useState} from "react";
import "./RightsManagerPanel.scss";
import {ReactComponent as SearchSvg} from "./../../assets/search.svg";
import {ReactComponent as Employee} from "./../../assets/add-employee.svg";
import {ReactComponent as Account} from "./../../assets/account.svg";
import {OutsideAlerter} from "../utils/OutSideAlerter/OutSideAlerter";

export const RightsManagerPanel: React.FC = memo(() => {
    useEffect(() => {

    },[])
    return (
        <div className={"rights-panel"}>
            <h2 className={"rights-panel__h2"}>Добавление доступа</h2>
            <div className={"rights-panel__content"}>
                <div className={"rights-panel__search-block"}>
                    <h3 className={"rights-panel__h3"}>Сотрудник</h3>
                    <InputDropdown/>
                </div>
                <div className={"rights-panel__employee-info"}>
                    <Employee className={"rights-panel__employee-info-svg"}/>
                    <p className={"rights-panel__employee-info-p"}>Вы можете найти сотрудника</p>
                    <p className={"rights-panel__employee-info-p"}> и добавить ему доступ</p>
                </div>
            </div>
        </div>
    );
})

const InputDropdown = () => {
    const FoundUsers = [1, 2, 3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,1, 2, 3,4,5,6,7,8,9,10,11,12,13,14,15,16,17].map(() => {
        return <div className={"rights-panel__search-block-dropdown-item"}>
            <Account/><span>Абобус</span>
        </div>
    })
    const [text, changeText] = useState("");
    const [isOpen, changeOpen] = useState(false);

    return <OutsideAlerter className={"rights-panel__search-block-label"} onOutsideClick={() =>  changeOpen(false)}><label className={"rights-panel__search-block-label"}>
        <input onClick={() => changeOpen(true)} className={"rights-panel__search-block-input"}
               placeholder={"Введите имя сотрудника"}/>
        <SearchSvg className={"rights-panel__search-block-svg"}/>
        {isOpen && <section onBlur={() => changeOpen(false)} className={"rights-panel__search-block-dropdown"}>
            {FoundUsers}
        </section>
        }
    </label>
    </OutsideAlerter>
}


