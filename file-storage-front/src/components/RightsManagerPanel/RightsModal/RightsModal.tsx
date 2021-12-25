import React, {memo, useEffect, useState} from "react";
import "./RightsModal.scss";
import Modal from "../../utils/Modal/Modal";
import {Button} from "../../utils/Button/Button";
import {useDispatch} from "react-redux";
import {managePanelSlice} from "../../../redux/managePanelSlice";
import {useAppSelector} from "../../../utils/hooks/reduxHooks";
import {Select} from "./Select";
import {fetchRightsUserById, fetchSetRightsUser} from "../../../redux/thunks/rightsThunks";

const {closeModal} = managePanelSlice.actions;

export const RightsModal: React.FC = memo(() => {
    const dispatch = useDispatch();
    const state = useAppSelector((state) => state);
    const {managePanel} = state;
    const {allRights, modal} = managePanel;
    const {name, idUser, rights} = modal;

    useEffect(() => {
        if (idUser) {
            dispatch(fetchRightsUserById(idUser))
        }
    },[])

    useEffect(() => {
        changeRights(rights)
    },[rights])

    const [newRights, changeRights] = useState<Array<number>>(rights);
    const options = allRights?.map(({id, name}, i) => ({label: name, value: id}));


    function close() {
        dispatch(closeModal());
    }

    function post(){
        if (!idUser)
            return;
        const grant = newRights.filter((e) => !rights.includes(e));
        const revoke = rights.filter((e) => !newRights.includes(e));
        if (grant.length === 0 && revoke.length === 0)
            return;
        dispatch(fetchSetRightsUser({userId: idUser, grant: grant, revoke:revoke}));
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
                                oldValues={rights}/>
                    </div>
                </div>
                <div className={"rights-modal__btns"}>
                    <Button type={"transparent"} onClick={close}>ОТМЕНА</Button>
                    <Button onClick={post}>ОК</Button>
                </div>
            </div>
        </Modal>
    );
})




