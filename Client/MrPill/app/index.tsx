import { Redirect } from 'expo-router';

export default function IndexScreen(userIsLoggedIn: boolean) {
    return <Redirect href={userIsLoggedIn? "/(login)/welcome": "/(home)/home"}/>;
}