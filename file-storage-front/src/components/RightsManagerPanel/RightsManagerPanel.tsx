import React, {Dispatch, memo, useEffect, useState} from "react";
import "./RightsManagerPanel.scss";
import {ReactComponent as SearchSvg} from "./../../assets/search.svg";
import {ReactComponent as Employee} from "./../../assets/add-employee.svg";
import {ReactComponent as Account} from "./../../assets/account.svg";
import {OutsideAlerter} from "../utils/OutSideAlerter/OutSideAlerter";
import {fetchAllUsers, fetchRightsDescription} from "../../redux/thunks/rightsThunks";
import {useDispatch} from "react-redux";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {managePanelSlice} from "../../redux/managePanelSlice";
import {RightsModal} from "./RightsModal/RightsModal";

const {openModal} = managePanelSlice.actions;
export const RightsManagerPanel: React.FC = memo(() => {
    const modal = useAppSelector(state => state.managePanel.modal);
    const dispatch = useDispatch();
    useEffect(() => {
        dispatch(fetchRightsDescription());
        dispatch(fetchAllUsers());
    }, [])

    return (<>

            <div className={"rights-panel"}>
                <h2 className={"rights-panel__h2"}>Добавление доступа</h2>
                <div className={"rights-panel__content"}>
                    <div className={"rights-panel__search-block"}>
                        <h3 className={"rights-panel__h3"}>Сотрудник</h3>
                        <InputDropdown dispatch={dispatch}/>
                    </div>
                    <div className={"rights-panel__employee-info"}>
                        <Employee className={"rights-panel__employee-info-svg"}/>
                        <p className={"rights-panel__employee-info-p"}>Вы можете найти сотрудника</p>
                        <p className={"rights-panel__employee-info-p"}> и добавить ему доступ</p>
                    </div>
                </div>
            </div>
            {modal.isOpen && <RightsModal/> }
        </>
    );
})

const InputDropdown: React.FC<{ dispatch: Dispatch<any> }> = memo(({dispatch}) => {
    const [text, changeText] = useState("");
    const [isOpen, changeOpen] = useState(false);
    const users = useAppSelector((state) => state.managePanel.users);
    const regexp = new RegExp(`${text}`, 'i');
    const FoundUsers = users?.filter((elem) => !!elem.name.match(regexp))?.map(({id, name}) => {
        function onClick() {
            dispatch(openModal({id}));
        }

        return <div onClick={onClick} className={"rights-panel__search-block-dropdown-item"} key={id}>
            <Account/><span>{name}</span>
        </div>
    })


    return <OutsideAlerter className={"rights-panel__search-block-label"}
                           onOutsideClick={() => changeOpen(false)}><label
        className={"rights-panel__search-block-label"}>
        <input onClick={() => changeOpen(true)} className={"rights-panel__search-block-input"}
               placeholder={"Введите имя сотрудника"}
               value={text} onChange={(e) => {
            changeText(e.target.value)
        }}/>
        <SearchSvg className={"rights-panel__search-block-svg"}/>
        {isOpen && users && users.length > 0 &&
        <section onBlur={() => changeOpen(false)} className={"rights-panel__search-block-dropdown"}>
            {FoundUsers}
        </section>
        }
    </label>
    </OutsideAlerter>
})


