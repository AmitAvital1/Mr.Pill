import * as FileSystem from 'expo-file-system';

export const Downloader = async (source_uri: string, dest_filename: string) => {

  FileSystem.downloadAsync(source_uri, FileSystem.documentDirectory + dest_filename)
  .then(({ uri }) => {
    console.log('Finished downloading to ', uri);
  })
  .catch(error => {
    console.error(error);
  });
  
}