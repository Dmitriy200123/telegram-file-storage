import React, {FC, memo} from "react";
import "./Profile.scss";
import {ReactComponent as Telegram} from "./../../assets/telegram.svg";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {fetchLogoutTelegram} from "../../redux/thunks/profileThunks";

export const Profile: FC = memo(() => {
    const dispatch = useAppDispatch();

    const avatar = useAppSelector((state) => state.profile.avatar);
    const name = useAppSelector((state) => state.profile.name);
    const hasTelegram = useAppSelector((state) => state.profile.hasTelegram);

    function onAuth() {
        let dataAuthGit = localStorage.getItem("oidc.user:https://git.66bit.ru:392b8f8766b8da0f5f64edaa50b89b633d302ab0fd7f94aa482d5510e1a97cda");
        if (!dataAuthGit)
            dataAuthGit = sessionStorage.getItem("oidc.user:https://git.66bit.ru:392b8f8766b8da0f5f64edaa50b89b633d302ab0fd7f94aa482d5510e1a97cda");
        if (dataAuthGit) {
            const json = JSON.parse(dataAuthGit);
            window.open(`https://t.me/sixty_six_bit_bot?start=${json?.access_token}`);
        }
    }

    function onLogoutTelegram() {
        dispatch(fetchLogoutTelegram())
    }

    return (<div className={"profile"}>
            <div className={"profile__block-img"}>
                <div className={"profile__img"}>
                    <img src={avatar ?? undefined} alt={"avatar"}/>
                </div>
                {
                    hasTelegram ? <span onClick={onLogoutTelegram}
                                        className={"profile__telegram"}><span>Отключить телеграмм</span> <Telegram/></span>
                        : <span onClick={onAuth}
                                className={"profile__telegram"}><span>Подключить телеграмм</span> <Telegram/></span>
                }
            </div>
            <div className={"profile__info"}>
                <h2 className={"profile__h2"}>Профиль</h2>
                <h3 className={"profile__h3"}>Имя</h3>
                <p className={"profile__p"}>{name}</p>
            </div>
        </div>
    );
})




