#include "lancerdecode.h"
#include "formats.h"
#include "logging.h"
#include <string.h>
#include <stdlib.h>

ld_pcmstream_t ld_pcmstream_open(ld_stream_t stream)
{
	unsigned char magic[32];
	/* Read in magic */
	stream->read(magic,32,1,stream);
	stream->seek(stream,0,LDSEEK_SET);
	/* Detect file type */
	//Riff
	if(memcmp(magic, "RIFF", 4) == 0) {
		return riff_getstream(stream);
	}
	//Ogg
	if(memcmp(magic, "OggS", 4) == 0) {
		return ogg_getstream(stream);
	}
	//Flac
	if(memcmp(magic, "fLaC", 4) == 0) {
		return flac_getstream(stream, 0);
	}
	//Mp3
	if(memcmp(magic,"ID3", 3) == 0) {
		return mp3_getstream(stream);
	}
	if(magic[0] == 0xFF && magic[1] == 0xFB) {
		return mp3_getstream(stream);
	}

	LOG_ERROR("Unable to detect file type");
	return NULL;
}

void ld_pcmstream_close(ld_pcmstream_t stream)
{
	stream->stream->close(stream->stream);
	free(stream);
}
