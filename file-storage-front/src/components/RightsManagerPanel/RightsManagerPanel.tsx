import React, {memo} from "react";
import "./RightsManagerPanel.scss";
import {ReactComponent as SearchSvg} from "./../../assets/search.svg";

export const RightsManagerPanel: React.FC = memo(() => {
    return (
        <div className={"rights-panel"}>
            <h2 className={"rights-panel__h2"}>Добавление доступа</h2>
            <div className={"rights-panel__content"}>
                <div className={"rights-panel__search-block"}>
                    <h3 className={"rights-panel__h3"}>Сотрудник</h3>
                    <label className={"rights-panel__search-block-label"} >
                        <input className={"rights-panel__search-block-input"} placeholder={"Введите имя сотрудника"}/>
                        <SearchSvg className={"rights-panel__search-block-svg"}/>
                    </label>
                </div>
                <div className={"rights-panel__employee-info"}>
                    Вы можете найти сотрудника и добавить ему доступ
                </div>
            </div>
        </div>
    );
})




