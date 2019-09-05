using System;

internal unsafe struct AVInputFormat
{
    /// <summary>A comma separated list of short names for the format. New names may be appended with a minor bump.</summary>
    public byte* name;
    /// <summary>Descriptive name for the format, meant to be more human-readable than name. You should use the NULL_IF_CONFIG_SMALL() macro to define it.</summary>
    public byte* long_name;
    /// <summary>Can use flags: AVFMT_NOFILE, AVFMT_NEEDNUMBER, AVFMT_SHOW_IDS, AVFMT_GENERIC_INDEX, AVFMT_TS_DISCONT, AVFMT_NOBINSEARCH, AVFMT_NOGENSEARCH, AVFMT_NO_BYTE_SEEK, AVFMT_SEEK_TO_PTS.</summary>
    public int flags;
    /// <summary>If extensions are defined, then no probe is done. You should usually not use extension format guessing because it is not reliable enough</summary>
    public byte* extensions;
    public AVCodecTag** codec_tag;
    /// <summary>AVClass for the private context</summary>
    public AVClass* priv_class;
    /// <summary>Comma-separated list of mime types. It is used check for matching mime types while probing.</summary>
    public byte* mime_type;
    /// <summary>*************************************************************** No fields below this line are part of the public API. They may not be used outside of libavformat and can be changed and removed at will. New public fields should be added right above. ****************************************************************</summary>
    public AVInputFormat* next;
    /// <summary>Raw demuxers store their codec ID here.</summary>
    public int raw_codec_id;
    /// <summary>Size of private data so that it can be allocated in the wrapper.</summary>
    public int priv_data_size;
    /// <summary>Tell if a given file has a chance of being parsed as this format. The buffer provided is guaranteed to be AVPROBE_PADDING_SIZE bytes big so you do not have to check for that unless you need more.</summary>
    public IntPtr read_probe;
    /// <summary>Read the format header and initialize the AVFormatContext structure. Return 0 if OK. &apos;avformat_new_stream&apos; should be called to create new streams.</summary>
    public IntPtr read_header;
    /// <summary>Read one packet and put it in &apos;pkt&apos;. pts and flags are also set. &apos;avformat_new_stream&apos; can be called only if the flag AVFMTCTX_NOHEADER is used and only in the calling thread (not in a background thread).</summary>
    public IntPtr read_packet;
    /// <summary>Close the stream. The AVFormatContext and AVStreams are not freed by this function</summary>
    public IntPtr read_close;
    /// <summary>Seek to a given timestamp relative to the frames in stream component stream_index.</summary>
    public IntPtr read_seek;
    /// <summary>Get the next timestamp in stream[stream_index].time_base units.</summary>
    public IntPtr read_timestamp;
    /// <summary>Start/resume playing - only meaningful if using a network-based format (RTSP).</summary>
    public IntPtr read_play;
    /// <summary>Pause playing - only meaningful if using a network-based format (RTSP).</summary>
    public IntPtr read_pause;
    /// <summary>Seek to timestamp ts. Seeking will be done so that the point from which all active streams can be presented successfully will be closest to ts and within min/max_ts. Active streams are all streams that have AVStream.discard &lt; AVDISCARD_ALL.</summary>
    public IntPtr read_seek2;
    /// <summary>Returns device list with it properties.</summary>
    public IntPtr get_device_list;
    /// <summary>Initialize device capabilities submodule.</summary>
    public IntPtr create_device_capabilities;
    /// <summary>Free device capabilities submodule.</summary>
    public IntPtr free_device_capabilities;
}

internal unsafe struct AVClass
{
    /// <summary>The name of the class; usually it is the same name as the context structure type to which the AVClass is associated.</summary>
    public byte* class_name;
    /// <summary>A pointer to a function which returns the name of a context instance ctx associated with the class.</summary>
    public IntPtr item_name;
    /// <summary>a pointer to the first option specified in the class if any or NULL</summary>
    public AVOption* option;
    /// <summary>LIBAVUTIL_VERSION with which this structure was created. This is used to allow fields to be added without requiring major version bumps everywhere.</summary>
    public int version;
    /// <summary>Offset in the structure where log_level_offset is stored. 0 means there is no such variable</summary>
    public int log_level_offset_offset;
    /// <summary>Offset in the structure where a pointer to the parent context for logging is stored. For example a decoder could pass its AVCodecContext to eval as such a parent context, which an av_log() implementation could then leverage to display the parent context. The offset can be NULL.</summary>
    public int parent_log_context_offset;
    /// <summary>Return next AVOptions-enabled child or NULL</summary>
    public IntPtr child_next;
    /// <summary>Return an AVClass corresponding to the next potential AVOptions-enabled child.</summary>
    public IntPtr child_class_next;
    /// <summary>Category used for visualization (like color) This is only set if the category is equal for all objects using this class. available since version (51 &lt;&lt; 16 | 56 &lt;&lt; 8 | 100)</summary>
    public AVClassCategory category;
    /// <summary>Callback to return the category. available since version (51 &lt;&lt; 16 | 59 &lt;&lt; 8 | 100)</summary>
    public IntPtr get_category;
    /// <summary>Callback to return the supported/allowed ranges. available since version (52.12)</summary>
    public IntPtr query_ranges;
}

internal unsafe struct AVOption
{
}

internal unsafe struct AVClassCategory
{
}

internal unsafe struct AVDictionary
{
}

internal struct AVCodecTag
{
    public int id;
    public uint tag;
}