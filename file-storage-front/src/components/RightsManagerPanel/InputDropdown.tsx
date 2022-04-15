import {FC, memo, useState} from "react";
import {OutsideAlerter} from "../utils/OutSideAlerter/OutSideAlerter";
import {useAppDispatch, useAppSelector} from "../../utils/hooks/reduxHooks";
import {managePanelSlice} from "../../redux/managePanelSlice";
import {ReactComponent as SearchSvg} from "./../../assets/search.svg";
import {ReactComponent as Account} from "./../../assets/account.svg";

const {openModal} = managePanelSlice.actions;

export const InputDropdown: FC = memo(() => {
    const dispatch = useAppDispatch();
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
