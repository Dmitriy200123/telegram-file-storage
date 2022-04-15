import React, {memo, useEffect, useState} from "react";
import "./RightsModal.scss";
import Modal, {hocModal} from "../../utils/Modal/Modal";
import {Button} from "../../utils/Button/Button";
import {useDispatch} from "react-redux";
import {managePanelSlice} from "../../../redux/managePanelSlice";
import {useAppSelector} from "../../../utils/hooks/reduxHooks";
import {Select} from "./Select";
import {fetchRightsCurrentUser, fetchRightsUserById, postSetRightsUser} from "../../../redux/thunks/rightsThunks";
import {Rights} from "../../../models/File";

const {closeModal} = managePanelSlice.actions;

export const RightsModal: React.FC = memo(() => {
    const dispatch = useDispatch();
    const allRights = useAppSelector((state) => state.managePanel.allRights);
    const name = useAppSelector((state) => state.managePanel.modal.name);
    const idUser = useAppSelector((state) => state.managePanel.modal.idUser);
    const rights = useAppSelector((state) => state.managePanel.modal.rights);


    useEffect(() => {
        if (idUser) {
            dispatch(fetchRightsUserById(idUser))
        }
    }, []);

    if (!idUser)
        return null;

    return (<Modal><RightsUI allRights={allRights} rights={rights} idUser={idUser} name={name}/></Modal>);
})


const RightsUI: React.FC<PropsType> = memo(({allRights, name, rights, idUser}) => {
    const dispatch = useDispatch();

    const [newRights, changeRights] = useState(rights);
    const options = allRights?.map(({id, name}) => ({label: name, value: id}));

    useEffect(() => {
        changeRights(rights)
    }, [rights])


    function close() {
        dispatch(closeModal());
    }

    async function post() {
        if (!idUser)
            return;
        const grant = newRights.filter((e) => !rights.includes(e));
        const revoke = rights.filter((e) => !newRights.includes(e));
        if (grant.length === 0 && revoke.length === 0) {
            close();
            return;
        }
        dispatch(postSetRightsUser({userId: idUser, grant: grant, revoke: revoke}));
    }

    return (
        <div className={"rights-modal"}>
            <div className={"rights-modal__close"} onClick={close}/>
            <h2 className={"rights-modal__h2"}>Настройки доступа сотрудника</h2>
            <div className={"rights-modal__name"}>
                <h3 className={"rights-modal__h3"}>Имя</h3>
                <p>{name || "Не указано"}</p>
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
    );
});

type PropsType = {
    allRights: { name: string, id: number }[],
    name: string | null | undefined,
    idUser: string,
    rights: Rights[]
}

