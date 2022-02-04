import React, {memo, useEffect} from "react";
import "./RightsManagerPanel.scss";
import {ReactComponent as Employee} from "./../../assets/add-employee.svg";
import {fetchAllUsers} from "../../redux/thunks/rightsThunks";
import {useDispatch} from "react-redux";
import {useAppSelector} from "../../utils/hooks/reduxHooks";
import {RightsModal} from "./RightsModal/RightsModal";
import {InputDropdown} from "./InputDropdown";

export const RightsManagerPanel: React.FC = memo(() => {
    const isOpen = useAppSelector(state => state.managePanel.modal.isOpen);
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(fetchAllUsers());
    }, [])

    return (<>
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
            {isOpen && <RightsModal/>}
        </>
    );
})

