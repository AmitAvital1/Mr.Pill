import { Redirect } from 'expo-router';
import DataHandler from '@/DataHandler';
import React, { useEffect } from 'react';

export default function IndexScreen() {

    const [isUserLoaded, setIsUserLoaded] = React.useState<boolean>(false);

    useEffect(()=>{

        if (isUserLoaded) return;
        setIsUserLoaded(true);

        const loadUser = async () => {
            await DataHandler.loadUserFromStorage();
        }

        loadUser();
    })

    return <Redirect href={"/(login)/welcome"}/>;
}