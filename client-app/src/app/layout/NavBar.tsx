import { Button, Container, Dropdown, Menu, Image, Icon } from "semantic-ui-react";
import { Link, NavLink } from "react-router-dom";
import { useStore } from "../stores/store";
import { observer } from "mobx-react-lite";

export default observer(function NavBar() {
    const { userStore: { user, logout }, notificationStore } = useStore();

    return (
        <Menu inverted fixed='top'>
            <Container>
                <Menu.Item>
                    <Button as={NavLink} to='/activities' primary content='Activities' />
                    <Button style={{ marginLeft: 10 }} as={NavLink} to='/createActivity' positive content='Create Activity' />
                </Menu.Item>

                <Menu.Item position='right'>
                    <Dropdown icon={null} trigger={<Icon name='bell' />} pointing='top left'>
                        <Dropdown.Menu>
                            {notificationStore.notifications.map(n => (
                                <Dropdown.Item key={n.id} text={n.message} />
                            ))}
                        </Dropdown.Menu>
                    </Dropdown>
                    <Image avatar spaced='right' src={user?.image || '/assets/user.png'} />
                    <Dropdown pointing='top left' text={user?.displayName}>
                        <Dropdown.Menu>
                            <Dropdown.Item as={Link} to={`/profiles/${user?.username}`} text='My Profile' icon='user' />
                            <Dropdown.Item onClick={logout} text='Logout' icon='power' />
                        </Dropdown.Menu>
                    </Dropdown>
                </Menu.Item>
            </Container>
        </Menu>
    )
})