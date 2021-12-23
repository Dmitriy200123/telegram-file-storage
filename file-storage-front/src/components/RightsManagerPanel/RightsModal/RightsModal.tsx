import React, {memo} from "react";
import "./RightsModal.scss";
import Modal from "../../utils/Modal/Modal";
import {Button} from "../../utils/Button/Button";
import {useDispatch} from "react-redux";
import {managePanelSlice} from "../../../redux/managePanelSlice";

const {closeModal} = managePanelSlice.actions;

export const RightsModal: React.FC = memo(() => {
    const dispatch = useDispatch();

    function close(){
        dispatch(closeModal());
    }
    return (
        <Modal>
            <div className={"rights-modal"}>
                <div className={"rights-modal__close"} onClick={close}/>
                <h2 className={"rights-modal__h2"}>Настройки доступа сотрудника</h2>
                <div className={"rights-modal__name"}>
                    <h3 className={"rights-modal__h3"}>Имя</h3>
                    <p>Иванова Екатерина Мирная</p>
                </div>
                <div className={"rights-modal__access"}>
                    <h3 className={"rights-modal__h3"}>Доступы</h3>
                </div>
                <div className={"rights-modal__btns"}>
                    <Button type={"transparent"} onClick={close}>ОТМЕНА</Button>
                    <Button>ОК</Button>
                </div>
            </div>
        </Modal>
    );
})




