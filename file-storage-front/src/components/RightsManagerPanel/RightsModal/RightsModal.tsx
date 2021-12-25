import React, {memo, useState} from "react";
import "./RightsModal.scss";
import Modal from "../../utils/Modal/Modal";
import {Button} from "../../utils/Button/Button";
import {useDispatch} from "react-redux";
import {managePanelSlice} from "../../../redux/managePanelSlice";
import {fetchData} from "../../../redux/api/api";
import {useAppSelector} from "../../../utils/hooks/reduxHooks";
import {Select} from "./Select";

const {closeModal} = managePanelSlice.actions;

export const RightsModal: React.FC = memo(() => {
    const dispatch = useDispatch();
    const state = useAppSelector((state) => state);
    const {managePanel, profile} = state;
    const {rights: currentRights} = profile;
    const {allRights, modal} = managePanel;
    const {name, idUser} = modal;

    const [newRights, changeRights] = useState<Array<number>>(currentRights);
    const options = allRights?.map(({id, name}, i) => ({label: name, value: i}));

    function close() {
        dispatch(closeModal());
    }

    return (
        <Modal>
            <div className={"rights-modal"}>
                <div className={"rights-modal__close"} onClick={close}/>
                <h2 className={"rights-modal__h2"}>Настройки доступа сотрудника</h2>
                <div className={"rights-modal__name"}>
                    <h3 className={"rights-modal__h3"}>Имя</h3>
                    <p>{name}</p>
                </div>
                <div className={"rights-modal__access"}>
                    <h3 className={"rights-modal__h3"}>Доступы</h3>
                    <div>
                        <Select name={"rights"} values={newRights} options={options} setValue={changeRights}
                                oldValues={currentRights}/>
                    </div>
                </div>
                <div className={"rights-modal__btns"}>
                    <Button type={"transparent"} onClick={close}>ОТМЕНА</Button>
                    <Button>ОК</Button>
                </div>
            </div>
        </Modal>
    );
})




