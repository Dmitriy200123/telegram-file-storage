import React, {memo} from "react";
import "./RightsManagerPanel.scss";
import {ReactComponent as SearchSvg} from "./../../assets/search.svg";
import {ReactComponent as Employee} from "./../../assets/add-employee.svg";
import {ReactComponent as Account} from "./../../assets/account.svg";
import {RightsModal} from "./RightsModal/RightsModal";

export const RightsManagerPanel: React.FC = memo(() => {
    // return <RightsModal/>
    return (
        <div className={"rights-panel"}>
            <h2 className={"rights-panel__h2"}>Добавление доступа</h2>
            <div className={"rights-panel__content"}>
                <div className={"rights-panel__search-block"}>
                    <h3 className={"rights-panel__h3"}>Сотрудник</h3>
                    <label className={"rights-panel__search-block-label"}>
                        <input className={"rights-panel__search-block-input"} placeholder={"Введите имя сотрудника"}/>
                        <SearchSvg className={"rights-panel__search-block-svg"}/>
                        <section className={"rights-panel__search-block-dropdown"}>
                            <div className={"rights-panel__search-block-dropdown-item"}>
                                <Account/><span>Абобус</span>
                            </div>
                        </section>
                    </label>
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




